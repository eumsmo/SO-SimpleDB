namespace SimpleDB  {
    public abstract class CRUD {
        public string Executar(Comando comando) {
            switch (comando.op) {
                case Operacao.Inserir:
                    if (comando.valor == null) return "invalid command: invalid value";

                    if (Inserir(comando.chave, comando.valor)) return comando.chave.ToString();
                    return "already exists";
                case Operacao.Remover:
                    if (Remover(comando.chave)) return "removed";
                    return "not found";
                case Operacao.Procurar:
                    string? valor = Buscar(comando.chave);

                    if (valor != null) return valor;
                    return "not found";
                case Operacao.Atualizar:
                    if (comando.valor == null) return "invalid command: invalid value";

                    if (Atualizar(comando.chave, comando.valor)) return comando.chave.ToString();
                    return "not found";
            }

            return "invalid command: unknown command";
        }

        public abstract bool Inserir(int chave, string valor);
        public abstract bool Remover(int chave);
        public abstract string? Buscar(int chave);
        public abstract bool Atualizar(int chave, string novoValor);
    }
}