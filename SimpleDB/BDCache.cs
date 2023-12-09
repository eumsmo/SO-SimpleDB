using System;
using System.Collections.Generic;

namespace SimpleDB {

    public class BDCache: CRUD {
        public Dictionary<int, string> cache;
        public int size;
        public string policy;

        public CRUD bancoDeDados;

        public BDCache(CRUD bancoDeDados, int size, string policy) {
            this.bancoDeDados = bancoDeDados;
            this.size = size;
            this.policy = policy;
            cache = new Dictionary<int, string>();
        }

        public override bool Inserir(int chave, string valor) {
            throw new NotImplementedException();
        }

        public override bool Remover(int chave) {
            throw new NotImplementedException();
        }

        public override string? Buscar(int chave) {
            throw new NotImplementedException();
        }

        public override bool Atualizar(int chave, string novoValor) {
            throw new NotImplementedException();
        }
    }
}