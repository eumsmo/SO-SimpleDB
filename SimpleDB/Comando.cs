namespace SimpleDB {
    public enum Operacao {
        Inserir,
        Remover,
        Procurar,
        Atualizar
    }

    public class Comando {
        public Operacao op;
        public int chave;
        public string? valor;
    }
}