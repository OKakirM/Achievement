using System.Globalization;
using System.Text;

namespace Achievement.Helpers
{
    /// <summary>
    /// Normalização de texto para pesquisas tolerantes a maiúsculas/acentos.
    /// O SQLite não ignora diacríticos no SQL, por isso a filtragem é feita em memória.
    /// </summary>
    public static class TextSearch
    {
        /// <summary>
        /// Devolve o texto em minúsculas e sem diacríticos (ex.: "Pokémon" → "pokemon").
        /// </summary>
        public static string Normalize(string? text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            var decomposed = text.ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(decomposed.Length);
            foreach (var c in decomposed)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
