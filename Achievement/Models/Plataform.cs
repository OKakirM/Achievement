namespace Achievement.Models
{
    public class Plataform
    {
        public class Plantaforma
        {
            /// <summary>
            /// Chave Primária
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// Tipo da plataforma (Console, PC, Portable ou Mobile)
            /// </summary>
            public PlataformType Type { get; set; }

            /// <summary>
            /// Nome do aparelho, ou seja, o nome do console, do pc, do portátil ou do dispositivo móvel
            /// </summary>
            public string Name { get; set; } = string.Empty;
        }
    }


    public enum PlataformType

    {
        Console,
        PC,
        Portable,
        Mobile
    }
}
