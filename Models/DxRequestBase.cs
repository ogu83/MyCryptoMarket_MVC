using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyCryptoMarket_MVC.Models
{
    public class DxRequestBase : DataSourceLoadOptionsBase
    {
        private const string _defaultSearchKey = "or";
        private const string _defaultLogicalOperator = "or";

        public bool IsSearch { get; set; }

        public List<DxFilter> DxFilter
        {
            get
            {
                var dxFilter = new List<DxFilter>();
                try
                {
                    if (Filter != null && Filter as IList<string> != null && (Filter as IList<string>).Any())
                    {
                        var filterStr = (Filter as IList<string>).FirstOrDefault();
                        dxFilter.AddRange(getDxFilter(filterStr));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0}, {1}", "Cannot parse DxFilter", ex.StackTrace);
                }

                return dxFilter;
            }
        }

        private List<DxFilter> getDxFilter(string filterStr)
        {
            var dxFilter = new List<DxFilter>();

            if (filterStr != null)
            {
                if (JsonConvert.DeserializeObject(filterStr) is JArray filterStrs)
                {
                    switch (filterStrs.First.Type)
                    {
                        case JTokenType.Array:
                            for (int i = 0; i < filterStrs.Count; i += 2)
                            {
                                var f_filter = (JsonConvert.DeserializeObject(filterStrs[i].ToString()) as JArray).Select(x => x.ToString()).ToList();
                                JArray f1 = null;
                                JArray f2 = null;
                                try
                                {
                                    f1 = JsonConvert.DeserializeObject(f_filter.FirstOrDefault()) as JArray;
                                    f2 = JsonConvert.DeserializeObject(f_filter.LastOrDefault()) as JArray;
                                }
                                catch (Exception ex)
                                {
                                    _ = ex; //Add a _ to Prevent Visual Studio non-sende warnings.
                                    //Oguz, nothing to do if not deserialized than it is a not a json string,
                                    //use as filter direct, no need to parse
                                }

                                if (f1 == null && f2 == null)
                                {
                                    dxFilter.Add(new DxFilter
                                    {
                                        Key = IsSearch 
                                            ? _defaultSearchKey 
                                            : i + 1 < filterStrs.Count && filterStrs[i+1].Type != JTokenType.Array
                                                ? filterStrs[i + 1].ToString()
                                                : _defaultLogicalOperator,
                                        Filter = (JsonConvert.DeserializeObject(filterStrs[i].ToString()) as JArray).Select(x => x.ToString()).ToList()
                                    });
                                }
                                else
                                {
                                    dxFilter.AddRange(getDxFilter(f_filter.FirstOrDefault()));
                                    dxFilter.AddRange(getDxFilter(f_filter.LastOrDefault()));
                                }
                            }
                            break;
                        case JTokenType.String:
                            if (filterStrs.First.Value<string>() == "!")
                            {
                                dxFilter.Add(new DxFilter
                                {
                                    Key = _defaultLogicalOperator,
                                    Filter = filterStrs.Last.Select(x => x.ToString()).ToList(),
                                    IsNot = true
                                });
                            }
                            else
                            {
                                dxFilter.Add(new DxFilter
                                {
                                    Key = _defaultLogicalOperator,
                                    Filter = filterStrs.Select(x => x.ToString()).ToList()
                                });
                            }
                            break;
                    }

                }
            }

            return dxFilter;
        }

        public Tuple<string, bool> OrderBy
        {
            get
            {
                var retval = "Id";
                var desc = false;

                if (this.Sort != null && this.Sort.Any())
                {
                    retval = this.Sort.FirstOrDefault().Selector;
                    desc = this.Sort.FirstOrDefault().Desc;
                }

                // if (this.Sort != null)
                // {
                //     retval = this.Sort.Selector;
                //     desc = this.Sort.Desc;    
                // }

                return new Tuple<string, bool>(retval, desc);
            }
        }
    }
}