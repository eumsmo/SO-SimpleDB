using System;
using System.Collections.Generic;

namespace SimpleDB {
    public class AgingRegistro: Registro {
        public int contador;

        public AgingRegistro(int chave, string valor) : base(chave, valor) {
            contador = 128; // Inicia com 10000000 (ou seja, 8 bits, ou 1 byte)
        }
    }

    public class AgingCache : BDCache {
        public AgingCache(CRUD bancoDeDados, int size) : base(bancoDeDados, size) {}

        protected override Registro CreateRegistro(int chave, string valor) {
            AgingRegistro registro = new AgingRegistro(chave, valor);
            registro.bitR = true;
            return registro;
        }

        protected override Registro GetRegistroASubstituir() {
            AgingRegistro candidato = (AgingRegistro) cache[0];
            int menorContador = candidato.contador;

            for (int i = 1; i < cache.Count; i++) {
                AgingRegistro reg = (AgingRegistro) cache[i];

                if (reg.contador < menorContador) {
                    candidato = reg;
                    menorContador = reg.contador;
                }
            }

            return candidato;
        }

        public override void Update() {
            // A cada update, o contador de todos os registros é dividido por 2
            foreach (Registro registro in cache) {
                AgingRegistro reg = (AgingRegistro) registro;
                reg.contador /= 2; // 10000000 -> 01000000

                // Se o bit R estiver setado, é adicionado um 1 no bit mais significativo (considerando que o contador é um byte)
                if (reg.bitR) {
                    reg.contador += 128; // 01000000 -> 11000000
                    reg.bitR = false;
                }
            }
        }

        // Utilizado para debug
        public override string ToString() {
            string text = "[";
            foreach (AgingRegistro registro in cache) {
                text += "{" + registro.chave + ",R:" + (registro.bitR?'1':'0') + ",Time:" + registro.contador + "} ";
            }
            text += "]";
            return text;
        }
    }
}