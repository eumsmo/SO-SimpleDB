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

        protected virtual void PrintCache() {
            Console.Write("[");
            foreach (Registro registro in cache) {
                Console.Write(registro.chave + " ");
            }
            Console.WriteLine("]");
        }

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

        Registro? GetRegistroInCache(int chave) {
            foreach (Registro registro in cache) {
                if (registro.chave == chave) {
                    return registro;
                }
            }

            return null;
        }

        Registro? GetRegistroInDatabase(int chave) {
            string? valor = bancoDeDados.Buscar(chave);

            if (valor == null) return null;

            Registro registro = CreateRegistro(chave, valor);
            return registro;
        }

        protected Registro? GetRegistro(int chave) {
            DownLeitores();
            Registro? registro = GetRegistroInCache(chave);
            UpLeitores();

            if (registro != null) {
                //Console.Write("Cache hit: ");
                //PrintCache();
                return registro;
            }

            registro = GetRegistroInDatabase(chave);
            if (registro != null) {
                //Console.Write("Cache miss: ");
                //PrintCache();
                InsertInCache(registro);
                //Console.Write("Cache after insert: ");
                //PrintCache();
                return registro;
            }

            return null;
        }

        protected void ExecutarRegistro(Registro registro) {
            if (registro.valor == null) bancoDeDados.Remover(registro.chave);
            else if (registro.novo) bancoDeDados.Inserir(registro.chave, registro.valor);
            else if(registro.bitM)  bancoDeDados.Atualizar(registro.chave, registro.valor);
        }

        void InsertInCache(Registro registro) {            
            
            if (cache.Count >= size) SubstituirRegistro(registro);
            else {
                DownEscritores();
                cache.Add(registro);
                UpEscritores();
            }
        }

        // Algortimos de substituição de página
        protected virtual Registro GetRegistroASubstituir() {
            return cache[0];
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

        protected virtual Registro CreateRegistro(int chave, string valor) {
            Registro registro = new Registro(chave, valor);
            registro.bitR = true;
            return registro;
        }

        public override bool Inserir(int chave, string valor) {
            Registro? registro = GetRegistro(chave);

            if (registro == null) {
                registro = CreateRegistro(chave, valor);
                registro.novo = true;
                registro.bitM = true;
                InsertInCache(registro);
            } else if (registro.valor == null) { // Marcado para deleção na cache
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
            if (registro == null || registro.valor == null) return false;

            DownEscritores();
            registro.valor = novoValor;
            registro.bitM = true;
            registro.bitR = true;
            UpEscritores();

            return true;
        }
    
        public override void Fechar() {
            foreach (Registro registro in cache) {
                ExecutarRegistro(registro);
            }

            bancoDeDados.Fechar();
        }

        public override void Update() {
            bancoDeDados.Update();
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