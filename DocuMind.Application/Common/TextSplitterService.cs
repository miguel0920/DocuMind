namespace DocuMind.Application.Common
{
    public class TextSplitterService : ITextSplitterService
    {
        public IEnumerable<string> SplitText(string text, int chunkSize, int chunkOverlap)
        {
            if (string.IsNullOrEmpty(text)) return [];
            if (chunkSize <= 0) throw new ArgumentException("El tamaño del chunk debe ser mayor a 0.");
            if (chunkOverlap >= chunkSize) throw new ArgumentException("El overlap no puede ser mayor o igual al tamaño del chunk.");

            var chunks = new List<string>();
            int textLength = text.Length;
            int i = 0;

            while (i < textLength)
            {
                // Validar que no nos pasemos del tamaño del texto
                int length = Math.Min(chunkSize, textLength - i);
                string chunk = text.Substring(i, length);
                chunks.Add(chunk);

                // Avanzar el puntero restando el overlap para mantener contexto
                i += chunkSize - chunkOverlap;

                // Si el avance ya no procesa más texto, rompemos el ciclo
                if (i >= textLength || length < chunkSize) break;
            }

            return chunks;
        }
    }
}