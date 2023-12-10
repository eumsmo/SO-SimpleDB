using System;
using System.Collections.Generic;

namespace SimpleDB {

    public class LRUCache : BDCache {
        public LRUCache(CRUD bancoDeDados, int size) : base(bancoDeDados, size) { }

        protected override Registro GetRegistroASubstituir() {
            Registro candidato = cache[0];
            int maiorPrioridade = CalcularPrioridade(candidato);

            for (int i = 1; i < cache.Count; i++) {
                Registro reg = cache[i];
                int prioridade = CalcularPrioridade(reg);

                if (prioridade > maiorPrioridade) {
                    candidato = reg;
                    maiorPrioridade = prioridade;
                }
            }

            return candidato;
        }

        int CalcularPrioridade(Registro registro) {
            /*
                A prioridade é dada pela seguinte tabela:

                | bitR | bitM | Prioridade |
                |------|------|------------|
                |  0   |  0   |     0      |
                |  0   |  1   |     1      |
                |  1   |  0   |     2      |
                |  1   |  1   |     3      |

                Quanto maior a prioridade, mais chances de ser substituído
            */
            return (registro.bitR ? 0 : 2) + (registro.bitM ? 0 : 1);
        }

        public override void Update() {
            // A cada update, o bit R de todos os registros é setado para false
            foreach (Registro registro in cache) {
                registro.bitR = false;
            }
        }

        // Utilizado para debug
        public override string ToString() {
            string text = "[";
            foreach (Registro registro in cache) {
                text += "{" + registro.chave + ",R:" + (registro.bitR?'1':'0') + ",M:" + (registro.bitM?'1':'0') + "} ";
            }
            text += "]";
            return text;
        }
    }
}