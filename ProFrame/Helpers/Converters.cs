using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Data;
using System.Windows;
using System.Collections;
using System.ComponentModel;
using System.Windows.Controls;

//namespace Salary
//{
//    public class IDToNameConverter : IValueConverter
//    {
//        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//        {
//            if (value is decimal || value is string || value is Boolean || value is double)
//            {
//                string[] st = parameter.ToString().Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
//                DataRow r = AppDataSet.Tables[st[0].ToUpper()].Rows.Find(value);
//                if (r != null)
//                    return r[st[1]].ToString();
//                else return "";
//            }
//            else return null;
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}

namespace ProFrame
{
    public class MultiSumConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!Array.TrueForAll(values, r => r != null && r != DependencyProperty.UnsetValue))
                return GridLength.Auto;
            double d = values.Sum(r => double.IsNaN((double)r) ? 0 : (double)r) + double.Parse((parameter == null ? "0" : parameter.ToString()), System.Globalization.CultureInfo.InvariantCulture);
            return new GridLength(d);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class DivideConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double c = double.Parse(parameter.ToString(), System.Globalization.CultureInfo.InvariantCulture);
            return ((double)value) * c;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MinusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double c = double.Parse(parameter.ToString(), System.Globalization.CultureInfo.InvariantCulture);
            return ((double)value) - c;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    

    public class SourceToNameConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!Array.TrueForAll(values, t => t != null && t != DependencyProperty.UnsetValue))
                return null;
            else
            {
                string propertyName = (string)parameter;
                CollectionViewSource c = values[0] as CollectionViewSource;
                DataView list = c.View.SourceCollection as DataView;
                DataRow rr = list.Table.Rows.Find(values[1]);
                if (rr != null)
                    return rr[propertyName];
                return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FindFieldConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!Array.TrueForAll(values, t => t != null && t != DependencyProperty.UnsetValue))
                return null;
            else
            {
                string propertyName = (string)parameter;
                IEnumerable l1 = values[0] as IEnumerable;
                IEnumerable<object> l = l1.OfType<object>();
                if (l.Count() > 0)
                {
                    PropertyDescriptor td = TypeDescriptor.GetProperties(l.ElementAt(0))[propertyName];
                    var t = from p in l
                            where td.GetValue(p).Equals(values[1])
                            select p;
                    return t.FirstOrDefault();
                }
                return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



    public class GetErrorElementConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return ValidationHelper.GetError(value as FrameworkElement);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DecimalToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == DBNull.Value || value == null || System.Convert.ToDecimal(value) == 0)
                return false;
            else return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool? d = (bool?)value;
            if (d.HasValue && d.Value)
                return 1m;
            else return 0m;
        }
    }
    public class InvertBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((bool?)value == false);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((bool?)value == false);
        }
    }
    public class InvertVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((Visibility?)value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((Visibility?)value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }
    }
    public class BoolToGridDetailsVisiblConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((bool?)value == false ? DataGridRowDetailsVisibilityMode.Collapsed : DataGridRowDetailsVisibilityMode.VisibleWhenSelected);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (DataGridRowDetailsVisibilityMode)value == DataGridRowDetailsVisibilityMode.Visible || (DataGridRowDetailsVisibilityMode)value == DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
        }
    }
    public class DecimalToVisiblityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value == null || (decimal)value == 0 ? DataGridRowDetailsVisibilityMode.Collapsed : DataGridRowDetailsVisibilityMode.Visible);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((DataGridRowDetailsVisibilityMode)value == DataGridRowDetailsVisibilityMode.Visible || (DataGridRowDetailsVisibilityMode)value == DataGridRowDetailsVisibilityMode.VisibleWhenSelected ? 1 : 0);
        }
    }
    public class DecimalInvertToVisiblityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value == null || (decimal)value == 0 ? Visibility.Visible : Visibility.Collapsed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((Visibility?)value == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed);
        }
    }

    public class ChildConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
                return (value as DataRowView).CreateChildView(parameter.ToString());
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IsNullConverter : IValueConverter 
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool fl = value == null || value == DBNull.Value;
            return fl;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Конвертер из даты в год и обратно.
    /// </summary>
    public class DateToYearConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || value == DBNull.Value || value == DependencyProperty.UnsetValue)
                return null;
            else
                return ((DateTime)value).Year;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || value == DBNull.Value || value == DependencyProperty.UnsetValue || string.IsNullOrWhiteSpace(value.ToString()))
                return null;
            else
                return new DateTime(int.Parse(value.ToString()), 1, 1);
        }
    }

}

//namespace Salary.View
//{
//    public class CollectionToSumConverter : IValueConverter
//    {
//        public string SumField
//        {
//            get;
//            set;
//        }
//        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//        {
//            return GetSum(value);
//        }

//        private decimal? GetSum(object value)
//        {
//            if (value != null)
//            {
//                IList<Object> dv = null;
//                IList<Object> vg = value as IList<Object>;
//                if (vg != null)
//                    dv = vg;
//                else
//                {
//                    Xceed.Wpf.DataGrid.DataGridCollectionView v1 = value as Xceed.Wpf.DataGrid.DataGridCollectionView;
//                    if (v1 != null)
//                        dv = (v1.SourceCollection as DataView).OfType<Object>().ToList();
//                    else
//                    {
//                        CollectionViewSource v = value as CollectionViewSource;
//                        if (v != null)
//                        {
//                            DataView vv=(v.Source as DataView);
//                            if (vv!=null) dv=dv.OfType<Object>().ToList();
//                        }
//                        else
//                        {
//                            DataView vv = value as DataView;
//                            if (vv != null)
//                                dv = vv.OfType<Object>().ToList();
//                        }
//                    }
//                }
//                if (dv != null)
//                {
//                    if (dv.Count>0)
//                    {
//                        Type tp = dv[0].GetType();
//                        if (tp == typeof(DataRowView))
//                            return dv.OfType<Object>().Sum(t => (t as DataRowView).Row.Field2<Decimal?>(SumField));
//                        else
//                            return dv.OfType<Object>().Sum(t => GetSum((t as CollectionViewGroup).Items));
//                    }
//                    else return 0;
//                }
//            }
//            return 0;
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }
//    }

//    /// <summary>
//    /// Самый полезный конвертер - суммирует поля коллекции по имени свойств через запятую, и возвращает массив в порядке перечисления суммируемых свойств
//    /// </summary>
//    public class CollectionsToSumsConverter : IMultiValueConverter
//    {
//        private string _sumField;
//        /// <summary>
//        /// Свойство суммирования по полям. Можно указать несколько полей через пробел
//        /// </summary>
//        public string SumField
//        {
//            get
//            {
//                return _sumField;
//            }
//            set
//            {
//                _sumField = value;
//                SummarizedFields = value.Split(new char[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
//            }
//        }
//        /// <summary>
//        /// Поля для суммирования значений
//        /// </summary>
//        private List<string> SummarizedFields
//        {
//            get;
//            set;
//        }

//        /// <summary>
//        /// Суммирование полей процедура
//        /// </summary>
//        /// <param name="value"></param>
//        /// <returns>возвращает массив данных</returns>
//        private decimal[] GetSum(object value)
//        {
//            decimal[] result = new decimal[SummarizedFields.Count];
//            if (value != null)
//            {
//                if (value is IEnumerable) // если это множество, то от каждого элемента пытаемся получить данные, иначе пытаемся у элемента данные получить
//                {
//                    foreach (object v in value as IEnumerable)
//                    {
//                        result = result.Zip(GetSum(v), (x, y) => x + y).ToArray();
//                    }
//                }
//                else
//                {
//                    if (value is CollectionViewGroup)
//                        result = GetSum((value as CollectionViewGroup).Items);
//                    else
//                    {
//                        PropertyDescriptorCollection pd = TypeDescriptor.GetProperties(value); // получаем все свойства у объекта
//                        for (int i=0;i<SummarizedFields.Count;++i)
//                        {
//                            string prop = SummarizedFields[i];
//                            if (pd[prop]!=null) // если есть такое свойство у объекта, то суммируем его
//                            {
//                                object temp = pd[prop].GetValue(value);
//                                result[i]+= temp==null||temp==DBNull.Value?0:(decimal)temp; // если значение не Null то суммируем его
//                            }
//                        }
//                    }
//                }
//            }
//            return result;
//        }

//        /// <summary>
//        /// Конвертируем значения в массив выходных значений
//        /// </summary>
//        /// <param name="values"></param>
//        /// <param name="targetType"></param>
//        /// <param name="parameter"></param>
//        /// <param name="culture"></param>
//        /// <returns></returns>
//        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//        {
//            decimal[] k = new decimal[SummarizedFields.Count];
//            foreach (object v in values)
//            {
//                k = k.Zip(GetSum(v), (x, y) => x + y).ToArray();
//            }
//            if (SummarizedFields.Count == 1)
//                return k[0];
//            else
//                return k;
//        }

//        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public class EqualConverter : IMultiValueConverter
//    {
//        public object DefaulValue
//        {
//            get;
//            set;
//        }
//        public object FalseValue
//        {
//            get;
//            set;
//        }
//        public object TrueValue
//        {
//            get;
//            set;
//        }
//        public Type CompareType
//        {
//            get;
//            set;
//        }
//        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//        {
//            if (values.Length > 1)
//            {
//                if (Array.TrueForAll(values, t=>t==DependencyProperty.UnsetValue || t==null))
//                    return DefaulValue;
//                if (values[0].Equals(values[1]))
//                    return TrueValue;
//                else
//                    return FalseValue;
//            }
//            else return DefaulValue;
//        }

//        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
