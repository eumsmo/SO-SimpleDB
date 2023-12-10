using System;
using System.Collections.Generic;

namespace SimpleDB {

    public class AgingRegistro: Registro {
        public int contador;

        public AgingRegistro(int chave, string valor) : base(chave, valor) {
            contador = 128; // 10000000
        }
    }

    public class AgingCache : BDCache {
        public AgingCache(CRUD bancoDeDados, int size) : base(bancoDeDados, size) {}

        protected override void PrintCache() {
            Console.Write("[");
            foreach (AgingRegistro registro in cache) {
                Console.Write("{" + registro.chave + ",R:" + (registro.bitR?'1':'0') + ",Time:" + registro.contador + "} ");
            }
            Console.WriteLine("]");
        }

        protected override Registro CreateRegistro(int chave, string valor) {
            AgingRegistro registro = new AgingRegistro(chave, valor);
            registro.bitR = true;
            return registro;
        }

        protected override void SubstituirPagina(Registro registro) {
            AgingRegistro? candidato = null;
            int index = -1;
            int menorContador = int.MaxValue;

            for (int i = 0; i < cache.Count; i++) {
                AgingRegistro reg = (AgingRegistro) cache[i];

                if (reg.contador < menorContador) {
                    candidato = reg;
                    index = i;
                    menorContador = reg.contador;
                }
            }

            if (candidato == null) return;

            cache.RemoveAt(index);
            ExecutarRegistro(candidato);
            cache.Add(registro);
        }

        public override void Update() {
            foreach (Registro registro in cache) {
                AgingRegistro reg = (AgingRegistro) registro;
                reg.contador /= 2; // 10000000 -> 01000000

                if (reg.bitR) {
                    reg.contador += 128; // 01000000 -> 11000000
                    reg.bitR = false;
                }
            }
        }
    }
}