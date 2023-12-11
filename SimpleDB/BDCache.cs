using System;
using System.Collections.Generic;
using System.Threading;

namespace SimpleDB {

    public class Registro {
        public int chave;
        public string? valor;
        public bool bitR;
        public bool bitM;
        public bool novo; // se é um registro novo (não está no BD)

        public Registro(int chave, string valor) {
            this.chave = chave;
            this.valor = valor;

            bitR = false;
            bitM = false;
            novo = false;
        }
    }

    public class BDCache: CRUD {
        public List<Registro> cache;
        public int size;

        protected Semaphore semaphore;
        protected Mutex mutex;
        protected int leitores;

        public CRUD bancoDeDados;

        public BDCache(CRUD bancoDeDados, int size) {
            this.bancoDeDados = bancoDeDados;
            this.size = size;
            cache = new List<Registro>(size);

            mutex = new Mutex();
            semaphore = new Semaphore(1, 1);
            leitores = 0;
        }

        #region Multithreading helpers

        protected void DownLeitores() {
            mutex.WaitOne();
            leitores++;
            if (leitores == 1) semaphore.WaitOne();
            mutex.ReleaseMutex();
        }

        protected void UpLeitores() {
            mutex.WaitOne();
            leitores--;
            if (leitores == 0) semaphore.Release();
            mutex.ReleaseMutex();
        }

        protected void DownEscritores() {
            semaphore.WaitOne();
        }

        protected void UpEscritores() {
            semaphore.Release();
        }

        #endregion

        #region Controle de cache

        protected virtual Registro CreateRegistro(int chave, string valor) {
            Registro registro = new Registro(chave, valor);
            registro.bitR = true;
            return registro;
        }

        Registro? GetRegistroInCache(int chave) {
            DownLeitores();
            foreach (Registro registro in cache) {
                if (registro.chave == chave) {
                    UpLeitores();
                    return registro;
                }
            }
            UpLeitores();
            return null;
        }

        Registro? GetRegistroInDatabase(int chave) {
            string? valor = bancoDeDados.Buscar(chave);

            if (valor == null) return null;

            Registro registro = CreateRegistro(chave, valor);
            return registro;
        }

        protected Registro? GetRegistro(int chave) {
            Registro? registro = GetRegistroInCache(chave);

            if (registro != null) {
                //Console.WriteLine("Cache hit: " + ToString());
                return registro;
            }

            // Se o registro não está na cache, tenta buscar no banco de dados
            registro = GetRegistroInDatabase(chave);
            if (registro != null) {
                //Console.WriteLine("Cache miss: " + ToString());
                InsertInCache(registro);
                //Console.WriteLine("Cache after insert: " + ToString());
                return registro;
            }

            return null;
        }

        protected void InsertInCache(Registro registro) {
            if (cache.Count >= size) SubstituirRegistro(registro);
            else {
                DownEscritores();
                cache.Add(registro);
                UpEscritores();
            }
        }

        protected virtual Registro GetRegistroASubstituir() {
            return cache[0]; // Por padrão, utiliza a politica FIFO
        }

        protected void SubstituirRegistro(Registro registro) {
            DownEscritores();
            Registro saiu = GetRegistroASubstituir();
            cache.Remove(saiu);
            cache.Add(registro);
            UpEscritores();

            Thread thread = new Thread(() => ExecutarRegistro(saiu));
            thread.Start();
        }
        
        // Executa um registro da cache no banco de dados
        protected void ExecutarRegistro(Registro registro) {
            if (registro.valor == null) bancoDeDados.Remover(registro.chave);
            else if (registro.novo) bancoDeDados.Inserir(registro.chave, registro.valor);
            else if(registro.bitM)  bancoDeDados.Atualizar(registro.chave, registro.valor);
        }

        #endregion

        #region Metodos do CRUD

        public override bool Inserir(int chave, string valor) {
            Registro? registro = GetRegistro(chave);

            if (registro == null) {
                registro = CreateRegistro(chave, valor);
                registro.novo = true;
                registro.bitM = true;
                InsertInCache(registro);
            } else if (registro.valor == null) {
                // Se o registro está marcado para deleção na cache, muda a operaçao para atualizar
                DownEscritores();
                registro.valor = valor;
                registro.bitM = true;
                registro.bitR = true;
                UpEscritores();
            } else return false;

            return true;
        }

        public override bool Remover(int chave) {
            Registro? registro = GetRegistro(chave);
            if (registro == null) return false;

            DownEscritores();
            if (registro.novo) {
                // Se o registro foi criado na cache mas não foi inserido no banco de dados, apenas remove da cache
                cache.Remove(registro);
                UpEscritores();

                return true;
            }

            registro.valor = null;
            registro.bitM = true;
            registro.bitR = true;
            UpEscritores();

            return true;
        }

        public override string? Buscar(int chave) {
            Registro? registro = GetRegistro(chave);
            if (registro == null) return null;

            DownEscritores();
            registro.bitR = true;
            UpEscritores();

            return registro.valor;
        }

        public override bool Atualizar(int chave, string novoValor) {
            Registro? registro = GetRegistro(chave);
            if (registro == null || registro.valor == null) return false; // Ignora se o registro não existe ou foi removido

            DownEscritores();
            registro.valor = novoValor;
            registro.bitM = true;
            registro.bitR = true;
            UpEscritores();

            return true;
        }
    
        public override void Fechar() {
            // Executa todos os registros da cache no banco de dados antes de fechar
            foreach (Registro registro in cache) {
                ExecutarRegistro(registro);
            }

            bancoDeDados.Fechar();
        }

        public override void Update() {
            // Na politica FIFO, não é necessário fazer nada
        }

        #endregion

        // Utilizado para debug
        public override string ToString() {
            string text = "[";
            foreach (Registro registro in cache) {
                text += registro.chave + " ";
            }
            text += "]";
            return text;
        }

        public static BDCache? GetCache(CRUD bancoDeDados, int size, string algoritmo) {
            if (size <= 0) return null;

            switch (algoritmo) {
                case "FIFO":
                    return new BDCache(bancoDeDados, size);
                case "LRU":
                    return new LRUCache(bancoDeDados, size);
                case "Aging":
                    return new AgingCache(bancoDeDados, size);
                default:
                    return null;
            }
        }
    }
}