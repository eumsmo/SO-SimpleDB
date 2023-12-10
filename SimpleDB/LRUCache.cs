using System;
using System.Collections.Generic;

namespace SimpleDB {

    public class LRUCache : BDCache {
        public LRUCache(CRUD bancoDeDados, int size) : base(bancoDeDados, size) { }

        protected override void PrintCache() {
            Console.Write("[");
            foreach (Registro registro in cache) {
                Console.Write("{" + registro.chave + ",R:" + (registro.bitR?'1':'0') + ",M:" + (registro.bitM?'1':'0') + "} ");
            }
            Console.WriteLine("]");
        }

        protected override void SubstituirPagina(Registro registro) {
            Registro? candidato = null;
            int index = -1;
            int maiorPrioridade = -1;

            for (int i = 0; i < cache.Count; i++) {
                Registro reg = cache[i];
                int prioridade = (reg.bitR ? 0 : 2) + (reg.bitM ? 0 : 1);

                if (prioridade > maiorPrioridade) {
                    candidato = reg;
                    index = i;
                    maiorPrioridade = prioridade;
                }
            }

            if (candidato == null) return;

            cache.RemoveAt(index);
            ExecutarRegistro(candidato);
            cache.Add(registro);
        }

        public override void Update() {
            foreach (Registro registro in cache) {
                registro.bitR = false;
            }
        }
    }
}