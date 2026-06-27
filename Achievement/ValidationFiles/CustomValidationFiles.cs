using System;
using System.Globalization;

namespace Achievement.ValidationFiles
{
    /// <summary>
    /// Classe de validação personalizada para arquivos, contendo constantes relacionadas a tamanho máximo de arquivo e caminhos de pastas para imagens
    /// </summary>
    public class CustomValidationFiles
    {
        // Formatos aceitos para a data de lançamento digitada pelo utilizador.
        public static readonly string[] _ReleaseDateFormats = { "yyyy/MM/dd", "dd/MM/yyyy", "MM/dd/yyyy" };

        // yyyy/MM/dd  ou  dd/MM/yyyy  ou  MM/dd/yyyy (estes dois últimos têm a mesma forma)
        public const string _ReleaseDateRegexPattern = @"^(\d{4}/\d{1,2}/\d{1,2}|\d{1,2}/\d{1,2}/\d{4})$";

        /// <summary>
        /// Tenta converter a data digitada num DateTime.
        /// https://stackoverflow.com/questions/52392114/use-datetime-tryparseexact-to-format-date-string-and-ignore-time
        /// </summary>
        public static bool TryParseReleaseDate(string? input, out DateTime date) =>
            DateTime.TryParseExact((input ?? string.Empty).Trim(), _ReleaseDateFormats,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out date);

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

        // - Pastas para imagens de perfil de utilizadores
        public const string _UsersAvatarFolder = "images/users/avatars";
        public const string _UsersBannerFolder = "images/users/banners";
    }
}
