using System;
using System.Collections.Generic;

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

        public CRUD bancoDeDados;

        public BDCache(CRUD bancoDeDados, int size) {
            this.bancoDeDados = bancoDeDados;
            this.size = size;
            cache = new List<Registro>(size);
        }

        protected virtual void PrintCache(){
            Console.Write("[");
            foreach(Registro registro in cache)
            {
                Console.Write(registro.chave + " ");
            }
            Console.WriteLine("]");
        }

        Registro? GetRegistroInCache(int chave) {
            foreach (Registro registro in cache) {
                if (registro.chave == chave) {
                    registro.bitR = true;
                    return registro;
                }
            }

            return null;
        }

        Registro? GetRegistroInDatabase(int chave) {
            string? valor = bancoDeDados.Buscar(chave);

            if (valor == null) {
                return null;
            }

            Registro registro = CreateRegistro(chave, valor);
            return registro;
        }

        protected Registro? GetRegistro(int chave) {
            Registro? registro = GetRegistroInCache(chave);

            if (registro != null) {
                //Console.WriteLine(">>> Cache hit");
                //PrintCache();
                return registro;
            }

            registro = GetRegistroInDatabase(chave);
            if(registro != null){
                //Console.WriteLine(">>> Cache miss");
                //PrintCache();
                InsertInCache(registro);
                //Console.WriteLine("Cache insert");
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

        protected bool InsertInCache(Registro registro) {
            // Se o registro já está na cache, não precisa colocar o valor novamente
            if(GetRegistroInCache(registro.chave) != null) return false;

            if(cache.Count >= size)
                SubstituirPagina(registro);
            else
                cache.Add(registro);
            
            return true;
        }

        //Algortimos de substituição de página
        protected virtual void SubstituirPagina(Registro registro){
            Registro saiu = cache[0];
            cache.RemoveAt(0);

            //Executar o registro em uma outra thread?
            ExecutarRegistro(saiu);

            cache.Add(registro);
        }

        void RemoveFromCache(Registro registro) {
            cache.Remove(registro);
        }

        protected virtual Registro CreateRegistro(int chave, string valor){
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
                registro.valor = valor;
                registro.bitM = true;
            } else return false;

            return true;
        }

        public override bool Remover(int chave) {
            Registro? registro = GetRegistro(chave);
            if (registro == null) return false;
            if (registro.novo) {
                RemoveFromCache(registro);
                return true;
            }

            registro.valor = null;
            registro.bitM = true;

            return true;
        }

        public override string? Buscar(int chave) {
            Registro? registro = GetRegistro(chave);
            if (registro == null) return null;

            return registro.valor;
        }

        public override bool Atualizar(int chave, string novoValor) {
            Registro? registro = GetRegistro(chave);
            if (registro == null || registro.valor == null) return false;

            registro.valor = novoValor;
            registro.bitM = true;

            return true;
        }
    
        public override void Fechar() {
            foreach (Registro registro in cache) {
                ExecutarRegistro(registro);
            }

            bancoDeDados.Fechar();
        }

        public static BDCache? GetCache(CRUD bancoDeDados, int size, string algoritmo){
            if(size <= 0) return null;

            switch(algoritmo){
                case "FIFO":
                    return new BDCache(bancoDeDados, size);
                case "LRU":
                    return new LRUCache(bancoDeDados, size);
                default:
                    return null;
            }
        }
    }
}