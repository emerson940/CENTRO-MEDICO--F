using CentroMedico.UI.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CentroMedico.UI.ViewModels
{
    public class ListadoViewModel<T> : ViewModelBase where T : class
    {
        private readonly Func<Task<List<T>>> _cargar;
        private bool _ocupado;
        public ObservableCollection<T> registros { get; } = new ObservableCollection<T>();
        public RelayCommand ComandoCargar { get; }
        public ListadoViewModel(Func<Task<List<T>>> cargar)
        {
            _cargar = cargar;
            ComandoCargar = new RelayCommand(Cargar, () => !_ocupado);
        }
        private T? _seleccionado;
        public T? seleccionado
        {
            get { return _seleccionado; }
            set { _seleccionado = value; OnPropertyChanged(nameof(seleccionado)); }
        }
        private string _mensaje = string.Empty;
        public string mensaje
        {
            get { return _mensaje; }
            set { _mensaje = value; OnPropertyChanged(nameof(mensaje)); }
        }
        private async void Cargar() { await CargarAsync(); }
        public async Task CargarAsync()
        {
            if (_ocupado) return;
            _ocupado = true;
            ComandoCargar.NotificarCanExecuteChanged();
            try
            {
                List<T> lista = await _cargar();
                seleccionado = null;
                registros.Clear();
                foreach (T registro in lista) registros.Add(registro);
                mensaje = $"Registros: {registros.Count}";
            }
            catch (Exception ex) { mensaje = "No se pudo cargar el listado: " + ex.Message; }
            finally
            {
                _ocupado = false;
                ComandoCargar.NotificarCanExecuteChanged();
            }
        }
    }
}
