using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace ReservasCanchas_Web.Components.Shared
{
    public partial class FormularioGenerico<TItem> : ComponentBase
    {
        [Parameter] public TItem? Model { get; set; }
        [Parameter] public EventCallback<TItem> OnValidSubmit { get; set; }
        [Parameter] public EventCallback OnCancel { get; set; }
        [Parameter] public bool IsEdit { get; set; } = false;

        protected PropertyInfo[] Props => typeof(TItem).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        protected object? GetValue(PropertyInfo prop) => prop.GetValue(Model);

        protected string? GetValueString(PropertyInfo prop)
        {
            var value = prop.GetValue(Model);
            return value?.ToString();
        }
        protected int? GetValueInt(PropertyInfo prop)
        {
            var value = prop.GetValue(Model);
            if (value == null) return null;
            if (value is int i) return i;
            if (int.TryParse(value.ToString(), out var result)) return result;
            return null;
        }
        protected decimal? GetValueDecimal(PropertyInfo prop)
        {
            var value = prop.GetValue(Model);
            if (value == null) return null;
            if (value is decimal d) return d;
            if (decimal.TryParse(value.ToString(), out var result)) return result;
            return null;
        }
        protected DateTime? GetValueDateTime(PropertyInfo prop)
        {
            var value = prop.GetValue(Model);
            if (value == null) return null;
            if (value is DateTime dt) return dt;
            if (DateTime.TryParse(value.ToString(), out var result)) return result;
            return null;
        }
        protected bool? GetValueBool(PropertyInfo prop)
        {
            var value = prop.GetValue(Model);
            if (value == null) return null;
            if (value is bool b) return b;
            if (bool.TryParse(value.ToString(), out var result)) return result;
            return null;
        }

        protected void SetValue(PropertyInfo prop, object? value)
        {
            if (Model == null) return;
            var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            if (value is null)
            {
                prop.SetValue(Model, null);
                return;
            }
            if (propType.IsEnum)
            {
                prop.SetValue(Model, Enum.Parse(propType, value.ToString()!));
            }
            else
            {
                prop.SetValue(Model, Convert.ChangeType(value, propType));
            }
        }

        protected async Task HandleValidSubmit()
        {
            if (Model != null)
                await OnValidSubmit.InvokeAsync(Model);
        }
    }
}