using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Windows.Controls;

namespace ProFrame
{
    public class CustomLocalizationManager : LocalizationManager
    {
        public override string GetStringOverride(string key)
        {
            switch (key)
            {
                case "GridViewGroupPanelText":
                    return "Перетащите заголовок столбца и оставьте его здесь, чтобы сгруппировать по этому столбцу.";
                //---------------------- RadGridView Filter Dropdown items texts:
                case "GridViewAlwaysVisibleNewRow":
                    return "Нажмите здесь, чтобы добавить новый элемент";
                case "GridViewClearFilter":
                    return "Убрать фильтр";
                case "GridViewFilter":
                    return "Фильтр";
                case "GridViewFilterAnd":
                    return "И";
                case "GridViewFilterContains":
                    return "Содержит";
                case "GridViewFilterDoesNotContain":
                    return "Не содержит";
                case "GridViewFilterEndsWith":
                    return "Заканчивается";
                case "GridViewFilterIsContainedIn":
                    return "Содержится в";
                case "GridViewFilterIsEqualTo":
                    return "Равно";
                case "GridViewFilterIsGreaterThan":
                    return "Больше, чем";
                case "GridViewFilterIsGreaterThanOrEqualTo":
                    return "Больше или равно";
                case "GridViewFilterIsNotContainedIn":
                    return "Не содержится в";
                case "GridViewFilterIsLessThan":
                    return "Меньше чем";
                case "GridViewFilterIsLessThanOrEqualTo":
                    return "Меньше или равно";
                case "GridViewFilterIsNotEqualTo":
                    return "Не равен";
                case "GridViewFilterMatchCase":
                    return "Учитывать регистр";
                case "GridViewFilterOr":
                    return "Или";
                case "GridViewFilterSelectAll":
                    return "Выбрать все";
                case "GridViewFilterShowRowsWithValueThat":
                    return "Показывать строки со значением, которое:";
                case "GridViewFilterStartsWith":
                    return "Начинается с";
                case "GridViewFilterIsNull":
                    return "Нулевой";
                case "GridViewFilterIsNotNull":
                    return "Не является нулевым";
                case "GridViewFilterIsEmpty":
                    return "Пусто";
                case "GridViewFilterIsNotEmpty":
                    return "Не пусто";
                case "GridViewFilterDistinctValueNull":
                    return "[ноль]";
                case "GridViewFilterDistinctValueStringEmpty":
                    return "[Пусто]";
                case "GridViewGroupPanelTopText":
                    return "Заголовок группы";
                case "GridViewGroupPanelTopTextGrouped":
                    return "Сгруппировано:";
                case "GridViewSearchPanelTopText":
                    return "Поиск полного текста";
                case "GridViewColumnsSelectionButtonTooltip":
                    return "Выберите столбцы";
                // -------------------
            }
            return base.GetStringOverride(key);
        }
    }
}
