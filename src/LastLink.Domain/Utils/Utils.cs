namespace LastLink.Domain.Utils
{
    public static class Utils
    {
        public static decimal CalculateValorLiquido(decimal bruto, decimal taxRate)
             => Math.Round(bruto * (1 - taxRate), 2, MidpointRounding.AwayFromZero);
    }
}
