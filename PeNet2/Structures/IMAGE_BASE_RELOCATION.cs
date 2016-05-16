﻿using System.Collections.Generic;
using System.Text;

namespace PeNet.Structures
{
    /// <summary>
    /// The IMAGE_BASE_RELOCATION structure holds information needed to relocate
    /// the image to another virtual address.
    /// </summary>
    public class IMAGE_BASE_RELOCATION
    {
        private readonly byte[] _buff;
        private readonly uint _offset;

        /// <summary>
        /// Create a new IMAGE_BASE_RELOCATION object.
        /// </summary>
        /// <param name="buff">PE binary as byte array.</param>
        /// <param name="offset">Offset to the relocation struct in the binary.</param>
        public IMAGE_BASE_RELOCATION(byte[] buff, uint offset)
        {
            _buff = buff;
            _offset = offset;
            ParseTypeOffsets();
        }

        /// <summary>
        ///     RVA of the relocation block.
        /// </summary>
        public uint VirtualAddress
        {
            get { return Utility.BytesToUInt32(_buff, _offset); }
            set { Utility.SetUInt32(value, _offset, _buff); }
        }

        /// <summary>
        /// SizeOfBlock-8 indicates how many TypeOffsets follow the SizeOfBlock.
        /// </summary>
        public uint SizeOfBlock
        {
            get { return Utility.BytesToUInt32(_buff, _offset + 0x4); }
            set { Utility.SetUInt32(value, _offset + 0x4, _buff); }
        }

        /// <summary>
        /// List with the TypeOffsets for the relocation block.
        /// </summary>
        public List<TypeOffset> TypeOffsets { get; private set; } = new List<TypeOffset>();

        private void ParseTypeOffsets()
        {
            for(uint i = 0; i < SizeOfBlock-8; i++)
            {
                TypeOffsets.Add(new TypeOffset(_buff, _offset + 8 + i * 2));
            }
        }

        /// <summary>
        ///     Convert all object properties to strings.
        /// </summary>
        /// <returns>String representation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder("IMAGE_BASE_RELOCATION\n");
            sb.Append(Utility.PropertiesToString(this, "{0,-10}:\t{1,10:X}\n"));
            TypeOffsets.ForEach(to => sb.AppendLine(to.ToString()));

            return sb.ToString();
        }

        /// <summary>
        /// Represents the type and offset in an 
        /// IMAGE_BASE_RELOCATION structure.
        /// </summary>
        public class TypeOffset
        {
            private readonly byte[] _buff;
            private readonly uint _offset;

            /// <summary>
            /// Create a new TypeOffset object.
            /// </summary>
            /// <param name="buff">PE binary as byte array.</param>
            /// <param name="offset">Offset of the TypeOffset in the array.</param>
            public TypeOffset(byte[] buff, uint offset)
            {
                _buff = buff;
                _offset = offset;
            }

            /// <summary>
            /// The type is described in the 4 lower bits of the
            /// TypeOffset word.
            /// </summary>
            public byte Type
            {
                get
                {
                    var to = Utility.BytesToUInt16(_buff, _offset);
                    return (byte)(to & 0xF);
                }
            }

            /// <summary>
            /// The offset is described in the 12 higher bits of the
            /// TypeOffset word.
            /// </summary>
            public ushort Offset
            {
                get
                {
                    var to = Utility.BytesToUInt16(_buff, _offset);
                    return (ushort)(to >> 4);
                }
            }

            /// <summary>
            ///     Convert all object properties to strings.
            /// </summary>
            /// <returns>String representation of the object</returns>
            public override string ToString()
            {
                var sb = new StringBuilder("TypeOffset\n");
                sb.Append(Utility.PropertiesToString(this, "{0,-10}:\t{1,10:X}\n"));

                return sb.ToString();
            }
        }
    }
}
