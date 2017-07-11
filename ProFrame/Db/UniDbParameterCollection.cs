using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace ProFrame
{
    public class UniDbParameterCollection : DbParameterCollection
    {
        DbParameterCollection m_parameters;
        List<UniParameter> list_params;
        internal UniDbParameterCollection(DbParameterCollection values)
        {
            list_params = new List<UniParameter>();
            m_parameters = values;
        }

        public override int Count
        {
            get
            {
                return m_parameters.Count;
            }
        }

        public override bool IsFixedSize
        {
            get
            {
                return m_parameters.IsFixedSize;
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                return m_parameters.IsReadOnly;
            }
        }

        public override bool IsSynchronized
        {
            get
            {
                return m_parameters.IsSynchronized;
            }
        }

        public override object SyncRoot
        {
            get
            {
                return m_parameters.SyncRoot;
            }
        }

        public override int Add(object value)
        {
            UniParameter v = (UniParameter)value;
            if (v.UniDbType == UniDbType.RefCursor)
                v.Direction = System.Data.ParameterDirection.Output;
            list_params.Add(v);
            return m_parameters.Add(v.InternalParameter);
        }

        /// <summary>
        /// Добавление параметра
        /// </summary>
        /// <param name="name">Имя параметра</param>
        /// <param name="type">Тип параметра</param>
        /// <param name="value">Значение параметра</param>
        /// <returns></returns>
        public UniParameter Add(string name, UniDbType type, ParameterDirection direction, string sourceColumn)
        {
            UniParameter v = new UniParameter(name, type, null) { Direction = direction, SourceColumn = sourceColumn };
            Add(v);
            return v;
        }
        
        /// <summary>
        /// Добавление параметра
        /// </summary>
        /// <param name="name">Имя параметра</param>
        /// <param name="type">Тип параметра</param>
        /// <param name="value">Значение параметра</param>
        /// <returns></returns>
        public UniParameter Add(string name, UniDbType type, object value)
        {
            UniParameter v = new UniParameter(name, type, value);
            Add(v);
            return v;
        }

        /// <summary>
        /// Добавление параметра запроса
        /// </summary>
        /// <param name="name">Имя параметра</param>
        /// <param name="type">тип параметра</param>
        /// <returns></returns>
        public UniParameter Add(string name, UniDbType type)
        {
            UniParameter v = new UniParameter(name, type, null);
            Add(v);
            return v;
        }

        public new UniParameter this[int index]
        {
            get
            {
                return list_params[index];
            }
        }
        public new UniParameter this[string name]
        {
            get
            {
                if (name == null)
                    throw new Exception("Неверное имя параметра. Оно не может быть пустым");
                foreach (UniParameter p in list_params)
                    if (p.ParameterName == name)
                        return p;
                return null;
            }
        }

        public override void AddRange(Array values)
        {
            m_parameters.AddRange(values);
        }

        public override void Clear()
        {
            m_parameters.Clear();
        }

        public override bool Contains(string value)
        {
            return m_parameters.Contains(value);
        }

        public override bool Contains(object value)
        {
            return m_parameters.Contains(value);
        }

        public override void CopyTo(Array array, int index)
        {
            m_parameters.CopyTo(array, index);
        }

        public override IEnumerator GetEnumerator()
        {
            return list_params.GetEnumerator();
        }

        public override int IndexOf(string parameterName)
        {
            return m_parameters.IndexOf(parameterName);
        }

        public override int IndexOf(object value)
        {
            return m_parameters.IndexOf(value);
        }

        public override void Insert(int index, object value)
        {
            list_params.Insert(index, (UniParameter)value);
            m_parameters.Insert(index, value);
        }

        public override void Remove(object value)
        {
            list_params.Remove((UniParameter)value);
            m_parameters.Remove(value);
        }

        public override void RemoveAt(string parameterName)
        {
            int k = -1;
            for (int i = 0; i < list_params.Count; ++i)
                if (parameterName == list_params[i].ParameterName)
                {
                    k = i;
                    break;
                }
            if (k>-1)
                list_params.RemoveAt(k);
            m_parameters.RemoveAt(parameterName);
        }

        public override void RemoveAt(int index)
        {
            list_params.RemoveAt(index);
            m_parameters.RemoveAt(index);
        }

        protected override DbParameter GetParameter(string parameterName)
        {
            if (parameterName == null)
                throw new Exception("Неверное имя параметра. Оно не может быть пустым");
            foreach (UniParameter p in list_params)
                if (p.ParameterName == parameterName)
                    return p;
            return null;
        }

        protected override DbParameter GetParameter(int index)
        {
            return list_params[index];
        }

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            for (int i=0;i<m_parameters.Count;++i)
                if (m_parameters[i].ParameterName == parameterName)
                {
                    m_parameters[i] = value;
                    list_params[i] = new UniParameter(value);
                    break;
                }
        }

        protected override void SetParameter(int index, DbParameter value)
        {
            list_params[index] = new UniParameter(value);
            m_parameters[index] = value;
        }
    }
}
