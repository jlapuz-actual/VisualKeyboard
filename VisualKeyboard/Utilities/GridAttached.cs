
namespace VisualKeyboard.Utilities
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    public class GridAttached : DependencyObject
    {
        //my regards to Rachel Lim for this code https://rachel53461.wordpress.com/2011/09/17/wpf-grids-rowcolumn-count-properties/

        #region DefinedRows AttachedProperty
        /// <summary>
        /// 
        /// 
        /// </summary>
        public static readonly DependencyProperty DefinedRowsProperty =
            DependencyProperty.RegisterAttached
            (
                "DefinedRows", //<PropertyName>

                /// property type 
                /// note: if bindable desired then best to keep to primitive types, 
                /// string preferable and parse later
                typeof( string ),
                typeof( GridAttached ),

                // <default value>, callback 
                // logic related to property placed here
                new PropertyMetadata( String.Empty, DefinedGridRowsChanged )
                );

        public static void SetDefinedRows( DependencyObject depObj, string value )
        //required set method, signature: "Set<PropertyName>(DependencyObject,value)>"
        {
            if ( depObj is null ) return;
            depObj.SetValue( DefinedRowsProperty, value );
        }

        public static string GetDefinedRows( DependencyObject depObj )
        //required get method, signature: "Get<PropertyName>(DependencyObject)"
        {
            if ( depObj is null ) return "";
            return depObj.GetValue( DefinedRowsProperty ).ToString();
        }

        public static void DefinedGridRowsChanged( DependencyObject depObj, DependencyPropertyChangedEventArgs e )
        // logic related to the property goes here
        {
            if ( depObj is null ) return;
            string [] RowSetting = ( (string)e.NewValue ).Split( ',' );
            if ( depObj is Grid || RowSetting.Length != 0 )
            {
                Grid grid = (Grid)depObj;
                grid.RowDefinitions.Clear();
                foreach ( string setting in RowSetting )
                {
                    GridLengthConverter converter = new GridLengthConverter();
                    RowDefinition rowDef = new RowDefinition()
                    { Height = (GridLength)converter.ConvertFrom( setting.Trim() ) };

                    grid.RowDefinitions.Add( rowDef );
                }

            }
        }
        #endregion

        #region DefinedColumns Attached Property
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty DefinedColumnProperty =
            DependencyProperty.RegisterAttached
            (
                "DefinedColumns",
                typeof( string ),
                typeof( GridAttached ),
                new PropertyMetadata( String.Empty, DefinedGridColumnChanged )
                );
        public static void SetDefinedColumns( DependencyObject depObj, string value )
        {
            if ( depObj is null ) return;
            depObj.SetValue( DefinedColumnProperty, value );

        }

        public static string GetDefinedColumns( DependencyObject depObj )
        {
            if ( depObj is null ) return "";

            return depObj.GetValue( DefinedColumnProperty ).ToString();
        }
        public static void DefinedGridColumnChanged( DependencyObject depObj, DependencyPropertyChangedEventArgs e )
        {
            if ( depObj is null ) return;
            string [] ColumnSetting = ( (string)e.NewValue ).Split( ',' );
            if ( depObj is Grid || ColumnSetting.Length != 0 )
            {
                Grid grid = (Grid)depObj;
                grid.ColumnDefinitions.Clear();
                foreach ( string setting in ColumnSetting )
                {
                    GridLengthConverter converter = new GridLengthConverter();
                    ColumnDefinition rowDef = new ColumnDefinition() { Width = (GridLength)converter.ConvertFrom( setting.Trim() ) };

                    grid.ColumnDefinitions.Add( rowDef );

                }
            }
        }

        #endregion
    }
}
