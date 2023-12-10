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

        protected override Registro GetRegistroASubstituir() {
            Registro candidato = cache[0];
            int maiorPrioridade = (candidato.bitR ? 0 : 2) + (candidato.bitM ? 0 : 1);

            for (int i = 1; i < cache.Count; i++) {
                Registro reg = cache[i];
                int prioridade = (reg.bitR ? 0 : 2) + (reg.bitM ? 0 : 1);

                if (prioridade > maiorPrioridade) {
                    candidato = reg;
                    maiorPrioridade = prioridade;
                }
            }

            return candidato;
        }

        public override void Update() {
            foreach (Registro registro in cache) {
                registro.bitR = false;
            }
        }
    }
}