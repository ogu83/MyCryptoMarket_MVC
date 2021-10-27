using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using LinqKit;
using MyCryptoMarket_MVC.Models;

namespace MyCryptoMarket_MVC.Helper
{
    public static class DynamicLinqExtensions
    {
        public static Expression<Func<TItem, bool>> PropertyEquals<TItem, TValue>(PropertyInfo property, TValue value)
        {
            var param = Expression.Parameter(typeof(TItem));
            var body = Expression.Equal(Expression.Property(param, property), Expression.Constant(value));
            return Expression.Lambda<Func<TItem, bool>>(body, param);
        }

        public static Expression<Func<TItem, bool>> PropertyStartsWith<TItem>(PropertyInfo propertyInfo, string value)
        {
            return propertyCompareMethod<TItem>(propertyInfo, value, "StartsWith");
        }

        public static Expression<Func<TItem, bool>> PropertyEndsWith<TItem>(PropertyInfo propertyInfo, string value)
        {
            return propertyCompareMethod<TItem>(propertyInfo, value, "EndsWith");
        }

        public static Expression<Func<TItem, bool>> PropertyContains<TItem>(PropertyInfo propertyInfo, string value)
        {
            return propertyCompareMethod<TItem>(propertyInfo, value, "Contains");
        }

        private static Expression<Func<TItem, bool>> propertyCompareMethod<TItem>(PropertyInfo propertyInfo, string value, string methodName)
        {
            var param = Expression.Parameter(typeof(TItem));

            var m = Expression.MakeMemberAccess(param, propertyInfo);
            var c = Expression.Constant(value, typeof(string));
            var mi = typeof(string).GetMethod(methodName, new Type[] { typeof(string) });
            var body = Expression.Call(m, mi, c);

            return Expression.Lambda<Func<TItem, bool>>(body, param);
        }

        public static IQueryable<T> SortBy<T>(this IQueryable<T> collection, string orderBy, bool isDescending = false, string instanceName = "")
            where T : class
        {
            if (!string.IsNullOrEmpty(instanceName))
            {
                orderBy = string.Join(".", instanceName, orderBy);
            }

            if (!isDescending)
            {
                return collection.OrderBy(orderBy);
            }
            else
            {
                return collection.OrderBy(orderBy + " descending");
            }
        }

        public static IQueryable<T> SortByMultiple<T>(this IQueryable<T> collection, string orderByFirst, string orderBySecond, bool isDescending = false)
            where T : class
        {
            if (collection is null){
                return collection;
            }

            if (!isDescending)
            {
                return collection.OrderBy(orderByFirst).ThenBy(orderBySecond);
            }
            else
            {
                return collection.OrderBy(orderByFirst + " descending").ThenBy(orderBySecond + " descending");
            }
        }

        public static IQueryable<T> SortByFullName<T>(this IQueryable<T> collection, bool isDescending = false)
            where T : class
        {
            if (!isDescending)
            {
                return collection.OrderBy("FirstName").ThenBy("LastName");
            }
            else
            {
                return collection.OrderBy("FirstName descending").ThenBy("LastName descending");
            }
        }

        public static Expression<Func<T, bool>> Like<T>(Expression<Func<T, string>> expr, string likeValue)
        {
            var paramExpr = expr.Parameters.First();
            var memExpr = expr.Body;

            if (likeValue == null || likeValue.Contains('%') != true)
            {
                Expression<Func<string>> valExpr = () => likeValue;
                var eqExpr = Expression.Equal(memExpr, valExpr.Body);
                return Expression.Lambda<Func<T, bool>>(eqExpr, paramExpr);
            }

            if (likeValue.Replace("%", string.Empty).Length == 0)
            {
                return PredicateBuilder.New<T>(true).DefaultExpression;
            }

            likeValue = Regex.Replace(likeValue, "%+", "%");

            if (likeValue.Length > 2 && likeValue.Substring(1, likeValue.Length - 2).Contains('%'))
            {
                likeValue = likeValue.Replace("[", "[[]").Replace("_", "[_]");
                Expression<Func<string>> valExpr = () => likeValue;
                var patExpr = Expression.Call(typeof(SqlFunctions).GetMethod("PatIndex", new[] { typeof(string), typeof(string) }), valExpr.Body, memExpr);
                var neExpr = Expression.NotEqual(patExpr, Expression.Convert(Expression.Constant(0), typeof(int?)));
                return Expression.Lambda<Func<T, bool>>(neExpr, paramExpr);
            }

            if (likeValue.StartsWith("%"))
            {
                if (likeValue.EndsWith("%") == true)
                {
                    likeValue = likeValue.Substring(1, likeValue.Length - 2);
                    Expression<Func<string>> valExpr = () => likeValue;
                    var containsExpr = Expression.Call(memExpr, typeof(string).GetMethod("Contains", new[] { typeof(string) }), arguments: valExpr.Body);
                    return Expression.Lambda<Func<T, bool>>(containsExpr, paramExpr);
                }
                else
                {
                    likeValue = likeValue.Substring(1);
                    Expression<Func<string>> valExpr = () => likeValue;
                    var endsExpr = Expression.Call(memExpr, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), valExpr.Body);
                    return Expression.Lambda<Func<T, bool>>(endsExpr, paramExpr);
                }
            }
            else
            {
                likeValue = likeValue.Remove(likeValue.Length - 1);
                Expression<Func<string>> valExpr = () => likeValue;
                var startsExpr = Expression.Call(memExpr, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), valExpr.Body);
                return Expression.Lambda<Func<T, bool>>(startsExpr, paramExpr);
            }
        }

        public static Expression<Func<T, bool>> AndLike<T>(this Expression<Func<T, bool>> predicate, Expression<Func<T, string>> expr, string likeValue)
        {
            var andPredicate = Like(expr, likeValue);
            if (andPredicate != null)
            {
                predicate = predicate.And(andPredicate.Expand());
            }

            return predicate;
        }

        public static Expression<Func<T, bool>> OrLike<T>(this Expression<Func<T, bool>> predicate, Expression<Func<T, string>> expr, string likeValue)
        {
            var orPredicate = Like(expr, likeValue);
            if (orPredicate != null)
            {
                predicate = predicate.Or(orPredicate.Expand());
            }

            return predicate;
        }

        public static IEnumerable<T> SortBy<T>(this IEnumerable<T> query, string name, bool isDescending = false)
        {
            var orderByColumns = name
                .Split(',')
                .Select(i => i.Trim())
                .ToList();

            foreach (var orderByColumn in orderByColumns)
            {
                query = query.SortBySingleColumn(orderByColumn, isDescending);
            }

            return query;
        }

        private static IEnumerable<T> SortBySingleColumn<T>(this IEnumerable<T> query, string name, bool isDescending = false)
        {
            var propInfo = GetPropertyInfo(typeof(T), name);
            var expression = GetOrderExpression(typeof(T), propInfo);

            var method = typeof(Enumerable).GetMethods().FirstOrDefault(m => m.Name == (!isDescending ? "OrderBy" : "OrderByDescending") && m.GetParameters().Length == 2);
            var genericMethod = method.MakeGenericMethod(typeof(T), propInfo.PropertyType);
            return (IEnumerable<T>)genericMethod.Invoke(null, new object[] { query, expression.Compile() });
        }

        private static PropertyInfo GetPropertyInfo(Type type, string name)
        {
            var properties = type.GetProperties();
            var matchedProperty = properties.FirstOrDefault(p => p.Name == name);
            if (matchedProperty == null)
            {
                throw new ArgumentException($"{name} property is not found in object of type {type.Name}.");
            }

            return matchedProperty;
        }

        private static LambdaExpression GetOrderExpression(Type objType, PropertyInfo property)
        {
            var paramExpression = Expression.Parameter(objType);
            var propertyAccess = Expression.PropertyOrField(paramExpression, property.Name);
            return Expression.Lambda(propertyAccess, paramExpression);
        }

        public static IQueryable<T> Filter<T>(IQueryable<T> query, List<DxFilter> dxFilter)
        {
            var orPredicate = PredicateBuilder.New<T>(false).DefaultExpression;
            var andPredicate = PredicateBuilder.New<T>(true).DefaultExpression;
            var anyOrFilter = false;
            var anyNotOrFilter = false;
            var anyAndFilter = false;
            var anyNotAndFilter = false;

            foreach (var f in dxFilter.Where(x => !x.IsDecimalFilter() && !x.IsLongFilter()))
            {
                var properties = typeof(T).GetProperties();
                var parameter = Expression.Parameter(typeof(T));
                MethodInfo method = null;

                bool hasFlagsAttribute = properties.Any(p => p.Name.Equals(f.Column) && p.PropertyType.CustomAttributes.Any(ca => ca.AttributeType == typeof(FlagsAttribute)));

                if (f.Key == "or")
                {
                    switch (f.FType)
                    {
                        case "contains":
                        default:
                            anyOrFilter = true;
                            method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                            break;
                        case "notcontains":
                            anyNotOrFilter = true;
                            method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                            break;
                        case "startswith":
                            anyOrFilter = true;
                            method = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
                            break;
                        case "endswith":
                            anyOrFilter = true;
                            method = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
                            break;
                        case "equals":
                        case "=":
                            anyOrFilter = true;
                            var propType = properties.FirstOrDefault(p => p.Name.Equals(f.Column)).PropertyType;
                            method = propType.GetMethod("Equals", new[] { propType });
                            break;
                        case "<>": //notequal
                            var notPropType = properties.FirstOrDefault(p => p.Name.Equals(f.Column)).PropertyType;
                            method = typeof(string).GetMethod("Equals", new[] { notPropType });
                            anyNotOrFilter = true;
                            break;
                        case ">":
                        case ">=":
                        case "<":
                        case "<=":
                            break;
                    }

                    if (!string.IsNullOrEmpty(f.Keyword))
                    {
                        var keywordLower = f.Keyword.ToLowerInvariant();

                        if (!hasFlagsAttribute && properties.Any(p => p.Name.Equals(f.Column) && p.PropertyType == typeof(string)))
                        {
                            var body = Expression.Call(Expression.Property(parameter, f.Column), method, Expression.Constant(keywordLower, typeof(string)));
                            orPredicate = orPredicate.Or(Expression.Lambda<Func<T, bool>>(anyNotOrFilter ? (Expression)Expression.Not(body) : (Expression)body, parameter));
                        }
                        else if (hasFlagsAttribute && properties.Any(p => p.Name.Equals(f.Column + "Description")))
                        {
                            var body = Expression.Call(Expression.Property(parameter, f.Column + "Description"), method, Expression.Constant(keywordLower, typeof(string)));
                            orPredicate = orPredicate.Or(Expression.Lambda<Func<T, bool>>(anyNotOrFilter ? (Expression)Expression.Not(body) : (Expression)body, parameter));
                        }
                        else if (!hasFlagsAttribute && properties.Any(p => p.Name.Equals(f.Column) && p.PropertyType == typeof(bool)))
                        {
                            var boolValue = bool.Parse(keywordLower);
                            orPredicate = orPredicate.Or(Expression.Lambda<Func<T, bool>>(
                                Expression.Call(Expression.Property(parameter, f.Column), method, Expression.Constant(boolValue, typeof(bool))),
                                parameter));
                        }
                    }
                }
                else if (f.Key == "and")
                {
                    switch (f.FType)
                    {
                        case "contains":
                        default:
                            anyAndFilter = true;
                            method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                            break;
                        case "notcontains":
                            anyNotAndFilter = true;
                            method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                            break;
                        case "startswith":
                            anyAndFilter = true;
                            method = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
                            break;
                        case "endswith":
                            anyAndFilter = true;
                            method = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
                            break;
                        case "equals":
                        case "=":
                            anyAndFilter = true;
                            method = typeof(string).GetMethod("Equals", new[] { typeof(string) });
                            break;
                        case "<>": //notequal
                            anyNotAndFilter = true;
                            method = typeof(string).GetMethod("Equals", new[] { typeof(string) });
                            break;
                        case ">":
                        case ">=":
                        case "<":
                        case "<=":
                            break;
                    }

                    if (!string.IsNullOrEmpty(f.Keyword))
                    {
                        var keywordLower = f.Keyword.ToLowerInvariant();

                        if (!hasFlagsAttribute && properties.Any(p => p.Name.Equals(f.Column)))
                        {
                            andPredicate = andPredicate.And(Expression.Lambda<Func<T, bool>>(
                                Expression.Call(Expression.Property(parameter, f.Column), method, Expression.Constant(keywordLower, typeof(string))),
                                parameter));
                        }
                        else if (hasFlagsAttribute && properties.Any(p => p.Name.Equals(f.Column + "Description")))
                        {
                            andPredicate = andPredicate.And(Expression.Lambda<Func<T, bool>>(
                                Expression.Call(Expression.Property(parameter, f.Column + "Description"), method, Expression.Constant(keywordLower, typeof(string))),
                                parameter));
                        }
                    }
                }
            }

            foreach (var f in dxFilter.Where(x => x.IsDecimalFilter() || x.IsLongFilter()))
            {
                var numericalPredicate = NumericalPredicate<T>(f);
                orPredicate = orPredicate.Or(numericalPredicate);
                andPredicate = andPredicate.And(numericalPredicate);
            }

            if (!anyOrFilter && !anyNotOrFilter && !anyAndFilter && !anyNotAndFilter)
            {
                return query.Where(andPredicate.Expand());
            }

            if (anyOrFilter || anyNotOrFilter)
            {
                query = query.Where(orPredicate.Expand());
            }

            if (anyAndFilter || anyNotAndFilter)
            {
                query = query.Where(andPredicate.Expand());
            }

            return query;
        }

        private static Expression<Func<T, bool>> NumericalPredicate<T>(DxFilter dxFilter)
        {
            var properties = typeof(T).GetProperties();
            var parameter = Expression.Parameter(typeof(T));
            var predicate = PredicateBuilder.New<T>(true).DefaultExpression;
            if (dxFilter != null && properties.Any(p => p.Name.Equals(dxFilter.Column)))
            {
                var property = properties.First(p => p.Name.Equals(dxFilter.Column));

                ConstantExpression rightExpr;
                if (property.PropertyType == typeof(long) || property.PropertyType == typeof(long?))
                {
                    rightExpr = Expression.Constant(dxFilter.KeywordAsLong, property.PropertyType);
                }
                else
                {
                    rightExpr = Expression.Constant(dxFilter.KeywordAsDecimal, property.PropertyType);
                }

                switch (dxFilter.FType)
                {
                    case "=":
                        predicate = Expression.Lambda<Func<T, bool>>(
                            Expression.Equal(Expression.Property(parameter, dxFilter.Column), rightExpr),
                            parameter);
                        break;
                    case "<>":
                        predicate = Expression.Lambda<Func<T, bool>>(
                            Expression.NotEqual(Expression.Property(parameter, dxFilter.Column), rightExpr),
                            parameter);
                        break;
                    case ">":
                        predicate = Expression.Lambda<Func<T, bool>>(
                            Expression.GreaterThan(Expression.Property(parameter, dxFilter.Column), rightExpr),
                            parameter);
                        break;
                    case ">=":
                        predicate = Expression.Lambda<Func<T, bool>>(
                            Expression.GreaterThanOrEqual(Expression.Property(parameter, dxFilter.Column), rightExpr),
                            parameter);
                        break;
                    case "<":
                        predicate = Expression.Lambda<Func<T, bool>>(
                            Expression.LessThan(Expression.Property(parameter, dxFilter.Column), rightExpr),
                            parameter);
                        break;
                    case "<=":
                        predicate = Expression.Lambda<Func<T, bool>>(
                            Expression.LessThanOrEqual(Expression.Property(parameter, dxFilter.Column), rightExpr),
                            parameter);
                        break;
                }
            }

            return predicate;
        }
    }
}