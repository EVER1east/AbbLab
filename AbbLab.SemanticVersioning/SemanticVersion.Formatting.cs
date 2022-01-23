using System;
using System.Text;

namespace AbbLab.SemanticVersioning
{
    public partial class SemanticVersion : IFormattable
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.SimpleAppend(Major).Append('.')
              .SimpleAppend(Minor).Append('.')
              .SimpleAppend(Patch);

            if (_preReleases.Length > 0)
            {
                sb.Append('-').SimpleAppend(_preReleases[0]);
                int length = _preReleases.Length;
                for (int i = 0; i < length; i++)
                    sb.Append(',').SimpleAppend(_preReleases[i]);
            }
            if (_buildMetadata.Length > 0)
            {
                sb.Append('+').Append(_buildMetadata[0]);
                int length = _buildMetadata.Length;
                for (int i = 0; i < length; i++)
                    sb.Append(',').Append(_buildMetadata[i]);
            }
            return sb.ToString();
        }
        public string ToString(string? format)
        {
            if (format is null or "G" or "g") return ToString();

            StringBuilder sb = new StringBuilder();
            char separator = default;
            static void FlushSeparator(StringBuilder sb, ref char separator)
            {
                if (separator == default) return;
                sb.Append(separator);
                separator = default;
            }

            int length = format.Length;
            int position = -1;
            while (++position < length)
            {
                int next;
                switch (format[position])
                {
                    case '\\': // treat the next character as a normal character
                        FlushSeparator(sb, ref separator);
                        sb.Append(++position < length ? format[position] : '\\');
                        break;
                    case '.':
                    case '-':
                    case '+':
                    case ' ': // separators are stored, in case the next identifier is omitted
                        FlushSeparator(sb, ref separator);
                        separator = format[position];
                        break;
                    case 'M':
                        next = position + 1;
                        if (next < length && format[next] is 'M')
                        {
                            position = next;
                            if (Major > 0) // 'MM' - include only if > 0
                            {
                                FlushSeparator(sb, ref separator);
                                sb.SimpleAppend(Major);
                            }
                        }
                        else // 'M' - include always
                        {
                            FlushSeparator(sb, ref separator);
                            sb.SimpleAppend(Major);
                        }
                        break;
                    case 'm':
                        next = position + 1;
                        if (next < length && format[next] is 'm')
                        {
                            position = next++;
                            if (next < length && format[next] is 'm')
                            {
                                position = next;
                                if (_buildMetadata is not null) // 'mmm' - metadata, if any
                                {
                                    int count = _buildMetadata.Length;
                                    FlushSeparator(sb, ref separator);
                                    sb.Append(_buildMetadata[0]);
                                    for (int i = 1; i < count; i++)
                                        sb.Append('.').Append(_buildMetadata[i]);
                                }
                            }
                            else if (Minor > 0) // 'mm' - include if > 0
                            {
                                FlushSeparator(sb, ref separator);
                                sb.SimpleAppend(Minor);
                            }
                        }
                        else // 'm' - include always
                        {
                            FlushSeparator(sb, ref separator);
                            sb.SimpleAppend(Minor);
                        }
                        break;
                    case 'p':
                        next = position + 1;
                        if (next < length && format[next] is 'p')
                        {
                            position = next++;
                            if (next < length && format[next] is 'p')
                            {
                                position = next;
                                if (_preReleases is not null) // 'ppp' - pre-releases, if any
                                {
                                    int count = _preReleases.Length;
                                    FlushSeparator(sb, ref separator);
                                    sb.SimpleAppend(_preReleases[0]);
                                    for (int i = 1; i < count; i++)
                                        sb.Append('.').SimpleAppend(_preReleases[i]);
                                }
                            }
                            else if (Patch > 0) // 'pp' - include if > 0
                            {
                                FlushSeparator(sb, ref separator);
                                sb.SimpleAppend(Patch);
                            }
                        }
                        else // 'p' - include always
                        {
                            FlushSeparator(sb, ref separator);
                            sb.SimpleAppend(Patch);
                        }
                        break;
                    default:
                        FlushSeparator(sb, ref separator);
                        break;
                }
            }

            return sb.ToString();
        }
        string IFormattable.ToString(string? format, IFormatProvider? _) => ToString(format);
    }
}
