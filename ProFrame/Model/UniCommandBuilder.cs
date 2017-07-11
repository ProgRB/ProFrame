using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ProFrame
{
    public class UniCommandBuilder
    {
       
        /// <summary>
        /// Получаем команду обновления данных согласно правилам
        /// </summary>
        /// <param name="schemaName">имя схемы</param>
        /// <param name="tableName">имя таблицы</param>
        /// <param name="columns">колонки по схеме</param>
        /// <returns>Возвращает команду</returns>
        public static UniDbCommand GetInsertCommand(string schemaName, string tableName, IEnumerable<UniSchemaColumn> columns)
        {
            string proc_parameters = string.Join(", ", columns.Select(r => $"p_{r.DbColumnName}=>:p_{r.DbColumnName}"));
            UniDbCommand InsertCommand = new UniDbCommand($"begin {schemaName}.{tableName}_UPDATE({proc_parameters});end;");
            InsertCommand.UpdatedRowSource = UpdateRowSource.OutputParameters;
            foreach (var p in columns)
            {
                UniParameter current_par = InsertCommand.Parameters.Add("p_" + p.DbColumnName, p.DbColumnType);
                current_par.SourceColumn = p.DbColumnName;
                if (p.IsPrimaryKey)
                {
                    current_par.Direction = ParameterDirection.InputOutput;
                    if (p.DbColumnType == UniDbType.Decimal || p.DbColumnType == UniDbType.Int)
                        current_par.DbType = DbType.Decimal;
                }
                else
                    current_par.Direction = ParameterDirection.Input;
            }
            return InsertCommand;
        }

        /// <summary>
        /// Получаем команду обновления данных согласно правилам
        /// </summary>
        /// <param name="schemaName">имя схемы</param>
        /// <param name="tableName">имя таблицы</param>
        /// <param name="columns">колонки по схеме</param>
        /// <returns>Возвращает команду</returns>
        public static UniDbCommand GetUpdateCommand(string schemaName, string tableName, IEnumerable<UniSchemaColumn> columns)
        {
            string proc_parameters = string.Join(", ", columns.Select(r => $"p_{r.DbColumnName}=>:p_{r.DbColumnName}"));
            UniDbCommand updateCommand = new UniDbCommand($"begin {schemaName}.{tableName}_UPDATE({proc_parameters});end;");
            updateCommand.UpdatedRowSource = UpdateRowSource.OutputParameters;
            foreach (var p in columns)
            {
                UniParameter current_par = updateCommand.Parameters.Add("p_" + p.DbColumnName, p.DbColumnType);
                current_par.SourceColumn = p.DbColumnName;
                if (p.IsPrimaryKey)
                {
                    current_par.Direction = ParameterDirection.InputOutput;
                    if (p.DbColumnType == UniDbType.Decimal || p.DbColumnType == UniDbType.Int)
                        current_par.DbType = DbType.Decimal;
                }
                else
                    current_par.Direction = ParameterDirection.Input;
            }
            return updateCommand;
        }

        public static UniDbCommand GetDeleteCommand(string schemaName, string tableName, IEnumerable<UniSchemaColumn> columns)
        {
            UniSchemaColumn primary_column = columns.Where(r => r.IsPrimaryKey).FirstOrDefault();
            if (primary_column == null)
                throw new Exception($"Невозможно реализовать команду удаления без первичного ключа. Таблица {tableName}");
            UniDbCommand cmd = new UniDbCommand();
            cmd = new UniDbCommand($"begin {schemaName}.{tableName}_DELETE(p_{primary_column.DbColumnName}=>:p_{primary_column.DbColumnName});end;");
            cmd.UpdatedRowSource = UpdateRowSource.OutputParameters;
            cmd.Parameters.Add("p_" + primary_column.DbColumnName, primary_column.DbColumnType, ParameterDirection.Input, primary_column.DbColumnName);
            return cmd;
        }

        /// <summary>
        /// Создает простую команду выбора всех данных из таблицы
        /// </summary>
        /// <param name="schemaName">имя схемы</param>
        /// <param name="tableName">имя таблицы</param>
        /// <returns></returns>
        public static UniDbCommand GetSelectCommand(string schemaName, string tableName)
        {
            return new UniDbCommand($"select * from {schemaName}.{tableName}");
        }

    }
}
