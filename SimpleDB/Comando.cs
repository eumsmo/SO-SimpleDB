namespace SimpleDB {
    public enum Operacao {
        Inserir,
        Remover,
        Procurar,
        Substituir
    }

    public class Comando {
        public Operacao op;
        public int chave;
        public string? valor;
    }
}