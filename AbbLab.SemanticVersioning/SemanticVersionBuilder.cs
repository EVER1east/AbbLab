using System;

namespace AbbLab.SemanticVersioning
{
    public class SemanticVersionBuilder
    {
        private int _major;
        private int _minor;
        private int _patch;

        public int Major
        {
            get => _major;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), value, Exceptions.MajorNegative);
                _major = value;
            }
        }
        public int Minor
        {
            get => _minor;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), value, Exceptions.MinorNegative);
                _minor = value;
            }
        }
        public int Patch
        {
            get => _patch;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), value, Exceptions.PatchNegative);
                _patch = value;
            }
        }



    }
}