using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MyCryptoMarket_MVC.Models
{
    public class DxFilter
    {
        public static readonly string[] NumberOperators = new string[] { "=", "<>", "<=", "<", ">=", ">" };
        public static readonly string[] NumberNotOperators = new string[] { "<>", "=", ">", ">=", "<", "<=" };
        public static readonly string[] LogicalOperators = new string[] { "or", "and" };

        public string Key { get; set; }

        public bool IsNot { get; set; } = false;

        public DxFilter KeyAsDxFilter
        {
            get
            {
                if (string.IsNullOrEmpty(Key) || LogicalOperators.Contains(Key))
                {
                    return null;
                }

                try
                {
                    var retval = JsonConvert.DeserializeObject<DxFilter>(Key);
                    return retval;
                }
                catch (System.Exception)
                {
                    return null;
                }
            }
        }

        public List<string> Filter { get; set; } = new List<string>();

        public string Column
        {
            get
            {
                if (this.Filter.Count > 0)
                {
                    return this.Filter[0];
                }

                return string.Empty;
            }

            set
            {
                if (this.Filter.Count > 0)
                {
                    this.Filter[0] = value;
                }
            }
        }

        public string FType
        {
            get
            {
                if (this.Filter.Count > 1)
                {
                    return IsNot ? NumberNotOperators[Array.IndexOf(NumberOperators, this.Filter[1])] : this.Filter[1];
                }

                return string.Empty;
            }

            set
            {
                if (this.Filter.Count > 1)
                {
                    this.Filter[1] = value;
                }
            }
        }

        public string Keyword
        {
            get
            {
                if (this.Filter.Count > 2)
                {
                    return this.Filter[2];
                }

                return string.Empty;
            }
        }

        public decimal? KeywordAsDecimal
        {
            get
            {
                if (decimal.TryParse(Keyword, out decimal val))
                {
                    return val;
                }
                else
                {
                    return null;
                }
            }
        }

        public long? KeywordAsLong
        {
            get
            {
                if (long.TryParse(Keyword, out long val))
                {
                    return val;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool IsDecimalFilter()
        {
            return (NumberOperators.Contains(FType) && decimal.TryParse(Keyword, out _));
        }

        public bool IsLongFilter()
        {
            return (NumberOperators.Contains(FType) && long.TryParse(Keyword, out _));
        }
    }
}
