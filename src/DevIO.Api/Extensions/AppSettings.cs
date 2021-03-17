namespace DevIO.Api.Extensions
{
    public class AppSettings
    {
        /// <summary>
        /// Chave de criptografia do token
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Validade do token
        /// </summary>
        public int ExpiracaoHoras { get; set; }

        /// <summary>
        /// Quem emite
        /// </summary>
        public string Emissor { get; set; }

        /// <summary>
        /// Quais URL o token é valido
        /// </summary>
        public string ValidoEm { get; set;}
    }
}