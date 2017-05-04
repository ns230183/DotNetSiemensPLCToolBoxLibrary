﻿using DotNetSiemensPLCToolBoxLibrary.DataTypes;
/*
 This implements a high level Wrapper between libnodave.dll and applications written
 in MS .Net languages.
 
 This ConnectionLibrary was written by Jochen Kuehner
 * http://jfk-solutuions.de/
 * 
 * Thanks go to:
 * Steffen Krayer -> For his work on MC7 decoding and the Source for his Decoder
 * Zottel         -> For LibNoDave

 The NCK part was written by J.Eger
 * 
 * Thanks go to:
 * Jochen Kuehner -> For his nice ConnectionLibrary
 * Thomas_v2.1    -> For the support of the telegram analyze

 WPFToolboxForSiemensPLCs is free software; you can redistribute it and/or modify
 it under the terms of the GNU Library General Public License as published by
 the Free Software Foundation; either version 2, or (at your option)
 any later version.

 WPFToolboxForSiemensPLCs is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU Library General Public License
 along with Libnodave; see the file COPYING.  If not, write to
 the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.  
*/
using System;

namespace DotNetSiemensPLCToolBoxLibrary.Communication
{
#if !IPHONE
    using Scm = System.ComponentModel;
    using SG = System.Globalization;
    [Scm.Editor(typeof(NckTagUITypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
#endif
    [Serializable]
    public class PLCNckTag : PLCTag
    {
        public PLCNckTag()
        {
        }

        public int NckArea { get; set; }
        public int NckUnit { get; set; }
        public int NckColumn { get; set; }
        public int NckLine { get; set; }
        public int NckModule { get; set; }
        public int NckLinecount { get; set; }

        public override bool DontSplitValue
        {
            get { return true; }
            set { }
        }

        public override string ToString()
        {
            string old = "";
            if (_oldvalues != null)
            {
                old = "   -- Old-Values: ";
                foreach (var oldvalue in _oldvalues)
                {
                    old += oldvalue.ToString() + ",";
                }
                old += "";
            }

            //string s = string.Format("0x{0},0x{1},0x{2},0x{3},0x{4},0x{5},{6},0x{7}", NckArea.ToString("X"), NckUnit.ToString("X"), NckColumn.ToString("X"), NckLine.ToString("X"), NckModule.ToString("X"), NckLinecount.ToString("X"), TagDataType, _internalGetSize().ToString("X"));
            NC_Var ncVar = new NC_Var(this);
            string s = string.Format("0x{0},0x{1},0x{2},0x{3},0x{4},0x{5},{6},0x{7},0x{8}", ncVar.SYNTAX_ID, ncVar.Bereich_u_einheit, ncVar.Spalte, ncVar.Zeile, ncVar.Bausteintyp, ncVar.ZEILENANZAHL, ncVar.Typ, ncVar.Laenge);

            if (Value != null)
            {
                return s + " = " + GetValueAsString() + old;
            }
            return s;
        }

        //Todo: look how long a NCK Request is???
        //internal override int _internalGetSize()
        //{
        //	return 1;
        //}
    }

    public class NC_Var
    {
        public NC_Var()
        {
        }

        public NC_Var(NC_Var var)
        {
            this.SYNTAX_ID = var.SYNTAX_ID;
            this.Bereich_u_einheit = var.Bereich_u_einheit;
            this.Spalte = var.Spalte;
            this.Zeile = var.Zeile;
            this.Bausteintyp = var.Bausteintyp;
            this.ZEILENANZAHL = var.ZEILENANZAHL;
            this.Typ = var.Typ;
            this.Laenge = var.Laenge;
        }

        public NC_Var(PLCNckTag nckTag)
        {
            if (nckTag != null)
            {
                this.SYNTAX_ID = 0x82;
                this.Bereich_u_einheit = (byte)(nckTag.NckArea << 5 | nckTag.NckUnit);
                this.Spalte = (UInt16)nckTag.NckColumn;
                this.Zeile = (UInt16)nckTag.NckLine;
                this.Bausteintyp = (byte)nckTag.NckModule;
                this.ZEILENANZAHL = (byte)nckTag.NckLinecount;
                this.Typ = GetNckType(nckTag.TagDataType);
                this.Laenge = (byte)nckTag._internalGetSize();
            }
        }

        public NC_Var(string ncVarSelector)
        {
            throw new NotImplementedException();
            //bereich_u_einheit = 0x40;
            //spalte = 0x78;
            //zeile = 0x1;
            //bausteintyp = 0x7F;
            //ZEILENANZAHL = 0x1;
            //typ = 0xF;
            //laenge = 0x8;
        }

        public NC_Var(int syntaxId, int bereich_u_einheit, int spalte, int zeile, int bausteinTyp, int zeilenAnzahl, int typ, int laenge)
        {
            this.SYNTAX_ID = (byte)syntaxId;
            this.Bereich_u_einheit = (byte)bereich_u_einheit;
            this.Spalte = (UInt16)spalte;
            this.Zeile = (UInt16)zeile;
            this.Bausteintyp = (byte)bausteinTyp;
            this.ZEILENANZAHL = (byte)zeilenAnzahl;
            this.Typ = (byte)typ;
            this.Laenge = (byte)laenge;
        }

        #region private Fields
        private byte syntaxId;
        private byte bereich_u_einheit;
        private UInt16 spalte;
        private UInt16 zeile;
        private byte bausteintyp;
        private byte zeilenanzahl;
        private byte typ;
        private byte laenge;
        #endregion

        #region public Fields
#if !IPHONE
        [Scm.CategoryAttribute("PlcNckTag")]
        [Scm.TypeConverter(typeof(ByteHexTypeConverter))]
#endif
        public byte SYNTAX_ID
        {
            get { return syntaxId; }
            set { syntaxId = value; }
        }

#if !IPHONE
        [Scm.CategoryAttribute("PlcNckTag")]
        [Scm.TypeConverter(typeof(ByteHexTypeConverter))]
#endif
        public byte Bereich_u_einheit
        {
            get { return bereich_u_einheit; }
            set { bereich_u_einheit = value; }
        }

#if !IPHONE
        [Scm.CategoryAttribute("PlcNckTag")]
        [Scm.TypeConverter(typeof(UInt16HexTypeConverter))]
#endif
        public UInt16 Spalte
        {
            get { return spalte; }
            set { spalte = value; }
        }

#if !IPHONE
        [Scm.CategoryAttribute("PlcNckTag")]
        [Scm.TypeConverter(typeof(UInt16HexTypeConverter))]
#endif
        public UInt16 Zeile
        {
            get { return zeile; }
            set { zeile = value; }
        }

#if !IPHONE
        [Scm.CategoryAttribute("PlcNckTag")]
        [Scm.TypeConverter(typeof(ByteHexTypeConverter))]
#endif
        public byte Bausteintyp
        {
            get { return bausteintyp; }
            set { bausteintyp = value; }
        }

#if !IPHONE
        [Scm.CategoryAttribute("PlcNckTag")]
        [Scm.TypeConverter(typeof(ByteHexTypeConverter))]
#endif
        public byte ZEILENANZAHL
        {
            get { return zeilenanzahl; }
            set { zeilenanzahl = value; }
        }

#if !IPHONE
        [Scm.CategoryAttribute("PlcNckTag")]
        [Scm.TypeConverter(typeof(ByteHexTypeConverter))]
#endif
        public byte Typ
        {
            get { return typ; }
            set { typ = value; }
        }

#if !IPHONE
        [Scm.CategoryAttribute("PlcNckTag")]
        [Scm.TypeConverter(typeof(ByteHexTypeConverter))]
#endif
        public byte Laenge
        {
            get { return laenge; }
            set { laenge = value; }
        }
        #endregion

        public PLCNckTag GetNckTag(int unit = 0, int rowOffset = 0)
        {
            //byte SYNTAX_ID = 0x82;
            byte bereich_u_einheit = (byte)(this.Bereich_u_einheit + unit);
            byte _bereich = (byte)((bereich_u_einheit & 0xE0) >> 5);         // (bereich_u_einheit & 2#11100000) schiebe rechts um 5 Bit
            byte _einheit = (byte)(bereich_u_einheit & 0x1F);                // & 2#00011111

            #region TYP
            TagDataType dataType = new TagDataType();
            int _ArraySize = 0;
            switch (this.Typ)
            {
                case 1:
                    dataType = TagDataType.Bool;
                    break;
                case 3:
                    dataType = TagDataType.Byte; //eNCK_LE_Int8;
                    break;
                case 4:
                    dataType = TagDataType.Word;
                    break;
                case 5:
                    dataType = TagDataType.Int; //eNCK_LE_Int16;
                    break;
                case 6:
                    dataType = TagDataType.Dword; //eNCK_LE_Uint32;
                    break;
                case 7:
                    dataType = TagDataType.Dint; //eNCK_LE_Int32;
                    break;
                case 8:
                    dataType = TagDataType.Float; //eNCK_LE_Float32;
                    break;
                case 14:
                    dataType = TagDataType.DateTime;
                    break;
                case 15:
                    dataType = TagDataType.LReal; //eNCK_LE_Float64;
                    break;
                case 18:
                    dataType = TagDataType.LInt; //eNCK_LE_Int64;
                    break;
                case 19:
                    //if (_bereich == 2)// && NC_Var.bausteintyp == 0x7f)
                    //    dataType = TagDataType.String; //eNCK_LE_String;
                    //else
                    dataType = TagDataType.CharArray; //eNCK_LE_String;

                    _ArraySize = this.Laenge;
                    break;
                default:
                    throw new Exception(string.Format("Unknown Type: {0}", this.Typ));
            }
            #endregion

            return new PLCNckTag() { TagDataType = dataType, NckArea = _bereich, NckUnit = _einheit, NckColumn = (int)this.Spalte, NckLine = (int)this.Zeile + rowOffset, NckModule = this.Bausteintyp, NckLinecount = this.ZEILENANZAHL, ArraySize = _ArraySize };
        }

        public static NC_Var GetNC_Var(PLCNckTag nckTag)
        {
            var ret = new NC_Var();

            if (nckTag != null)
            {
                ret.SYNTAX_ID = 0x82;
                ret.Bereich_u_einheit = (byte)(nckTag.NckArea << 5 | nckTag.NckUnit);
                ret.Spalte = (UInt16)nckTag.NckColumn;
                ret.Zeile = (UInt16)nckTag.NckLine;
                ret.Bausteintyp = (byte)nckTag.NckModule;
                ret.ZEILENANZAHL = (byte)nckTag.NckLinecount;
                ret.Typ = GetNckType(nckTag.TagDataType);
                ret.Laenge = (byte)nckTag._internalGetSize();
            }
            return ret;
        }

        public static byte GetNckType(TagDataType type)
        {
            switch (type)
            {
                case TagDataType.Bool:
                    return 1;
                case TagDataType.Byte:
                    return 3;
                case TagDataType.Word:
                    return 4;
                case TagDataType.Int:
                    return 5;
                case TagDataType.Dword:
                    return 6;
                case TagDataType.Dint:
                    return 7;
                case TagDataType.Float:
                    return 8;
                case TagDataType.DateTime:
                    return 14;
                case TagDataType.LReal:
                    return 15;
                case TagDataType.LInt:
                    return 18;
                case TagDataType.String:
                case TagDataType.CharArray:
                    return 19;
                default:
                    return 0;
            }
        }


#if !IPHONE
        public class ByteHexTypeConverter : Scm.TypeConverter
        {
            public override bool CanConvertFrom(Scm.ITypeDescriptorContext context, Type sourceType)
            {
                return sourceType == typeof(string);
            }

            public override object ConvertFrom(Scm.ITypeDescriptorContext context, SG.CultureInfo culture, object value)
            {
                if (value is string)
                {
                    string s = (string)value;
                    if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    {
                        s = s.Substring(2);

                        return Byte.Parse(s, SG.NumberStyles.HexNumber, culture);
                    }
                    return Byte.Parse(s, SG.NumberStyles.AllowThousands, culture);
                    //byte s = (byte)value;
                    //return value.ToString("X");
                }

                return base.ConvertFrom(context, culture, value);
            }

            public override object ConvertTo(Scm.ITypeDescriptorContext context, SG.CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof(string) && value.GetType() == typeof(byte))
                    return "0x" + ((byte)value).ToString("X2", culture);

                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        public class UInt16HexTypeConverter : Scm.TypeConverter
        {
            public override bool CanConvertFrom(Scm.ITypeDescriptorContext context, Type sourceType)
            {
                return sourceType == typeof(string);
            }

            public override object ConvertFrom(Scm.ITypeDescriptorContext context, SG.CultureInfo culture, object value)
            {
                if (value is string)
                {
                    string s = (string)value;
                    if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    {
                        s = s.Substring(2);

                        return UInt16.Parse(s, SG.NumberStyles.HexNumber, culture);
                    }
                    return UInt16.Parse(s, SG.NumberStyles.AllowThousands, culture);
                    //byte s = (byte)value;
                    //return value.ToString("X");
                }

                return base.ConvertFrom(context, culture, value);
            }

            public override object ConvertTo(Scm.ITypeDescriptorContext context, SG.CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof(string) && value.GetType() == typeof(UInt16))
                    return "0x" + ((UInt16)value).ToString("X4", culture);

                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
#endif
    }
}
