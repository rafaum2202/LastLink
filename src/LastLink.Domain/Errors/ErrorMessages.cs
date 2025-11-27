using FluentResults;

namespace LastLink.Domain.Errors
{
    public class ErrorMessages
    {
        public static readonly Error VALOR_SOLICITADO_INVALIDO = new ("Valor solicitado deve ser maior que R$ 100,00!");
        public static readonly Error CREATOR_COM_SOLICITACAO_PENDENTE = new ("Creator já possui uma solicitação pendente!");
        public static readonly Error ERRO_AO_CRIAR_SOLICITACAO = new("Erro ao criar solicitação!");
        public static readonly Error CREATOR_SEM_SOLICITACOES = new("Creator sem solicitações!");
        public static readonly Error STATUS_INVALIDO = new("Status informado é inválido");
        public static readonly Error SOLICITACAO_NAO_ENCONTRADA = new("Solicitação não encontrada!");
        public static readonly Error SOLICITACAO_FINALIZADA = new("Solicitação já se encontra finalizada!");
    }
}
