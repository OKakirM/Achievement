namespace Achievement.ValidationFiles
{
    /// <summary>
    /// Classe de validação personalizada para arquivos, contendo constantes relacionadas a tamanho máximo de arquivo e caminhos de pastas para imagens
    /// </summary>
    public class CustomValidationFiles
    {
        // Tamanho máximo do arquivo
        // - Default: 10 MB
        public const int _MaxFileSize = 10 * (1024 * 1024); // 10 MB

        // Tipos de conteúdo de arquivo permitidos
        public const string _FileContentTypeJpg = "image/jpeg";
        public const string _FileContentTypePng = "image/png";

        // Caminhos das pastas para imagens
        // - Pastas para imagens de jogos
        public const string _GamesCoverFolder = "images/games/covers";
        public const string _GamesBannerFolder = "images/games/banners";
        public const string _GamesCoverDefaultImage = "images/games/covers/_default_cover.png";
        public const string _GamesBannerDefaultImage = "images/games/banners/_default_banner.png";
    }
}
