using System;

/// <summary>
/// Convert value to specify data type.
/// </summary>
public class DataManager
{
    #region "ConvertToByte"
    /// <summary>
    /// Convert value to Byte Type
    /// </summary>
    /// <param name="value">Value that you want to convert</param>
    /// <returns>Converted Value</returns>
    public static Byte ConvertToByte( object value )
    {
        return ConvertToByte( value, Byte.MinValue );
    }

    /// <summary>
    /// Convert value to Byte Type
    /// </summary>
    /// <param name="value">Value that you want to convert</param>
    /// <param name="defaultValue">Default value will be used when convert value is null or System.DBNull</param>
    /// <returns>Converted Value</returns>
    public static Byte ConvertToByte( object value, Byte defaultValue )
    {
        try
        {
            if ( value == null || value is DBNull )
            {
                return defaultValue;
            }
            return Convert.ToByte( value );
        }
        catch
        {
            return defaultValue;
        }
    }
    #endregion

    #region "ConvertToArrayBytes"
    /// <summary>
    /// Convert value to Array of Bytes
    /// </summary>
    /// <param name="value">Value that you want to convert</param>
    /// <returns>Converted Value</returns>
    public static Byte[] ConvertToArrayBytes( object value )
    {
        return ConvertToArrayBytes( value, Convert.FromBase64String( String.Empty ) );
    }

    /// <summary>
    /// Convert value to Array of Bytes
    /// </summary>
    /// <param name="value">Value that you want to convert</param>
    /// <param name="defaultValue">Default value will be used when convert value is null or System.DBNull</param>
    /// <returns>Converted Value</returns>
    public static Byte[] ConvertToArrayBytes( object value, Byte[] defaultValue )
    {
        try
        {
            if ( value == null || value is DBNull )
            {
                return defaultValue;
            }
            return Convert.FromBase64String( value.ToString() );
        }
        catch
        {
            return defaultValue;
        }
    }
    #endregion

    #region "ConvertToChar"
    /// <summary>
    /// Convert value to Char Type
    /// </summary>
    /// <param name="value">Value that you want to convert</param>
    /// <returns>Converted Value</returns>
    public static char ConvertToChar( object value )
    {
        return ConvertToChar( value, char.MinValue );
    }

    /// <summary>
    /// Convert value to Char Type
    /// </summary>
    /// <param name="value">Value that you want to convert</param>
    /// <param name="defaultValue">Default value will be used when convert value is null or System.DBNull</param>
    /// <returns>Converted Value</returns>
    public static char ConvertToChar( object value, char defaultValue )
    {
        try
        {
            if ( value == null || value is DBNull )
            {
                return defaultValue;
            }
            return Convert.ToChar( value );
        }
        catch
        {
            return defaultValue;
        }
    }
    #endregion

    #region "ConvertToShort"
    /// <summary>
    /// Convert value to Short (Int16) Type
    /// </summary>
    /// <param name="value">Value that you want to convert</param>
    /// <returns>Converted Value</returns>
    public static short ConvertToShort( object value )
    {
        return ConvertToShort( value, (short)0 );
    }

    /// <summary>
    /// Convert value to Short (Int16) Type
    /// </summary>
    /// <param name="value">Value that you want to convert</param>
    /// <param name="defaultValue">Default value will be used when convert value is null or System.DBNull</param>
    /// <returns>Converted Value</returns>
    public static short ConvertToShort( object value, short defaultValue )
    {
        try
        {
            if ( value == null || value is DBNull )
            {
                return defaultValue;
            }
            return Convert.ToInt16( value );
        }
        catch
        {
            return defaultValue;
        }
    }
    #endregion

    #region "ConvertToInteger"
    /// <summary>
    /// Convert value to Integer (Int32) Type
    /// </summary>
    /// <param name="value">Value that you want to convert</param>
    /// <returns>Converted Value</returns>
    public static int ConvertToInteger( object value )
    {
        return ConvertToInteger( value, 0 );
    }

    /// <summary>
    /// Convert value to Integer (Int32) Type
    /// </summary>
    /// <param name="value">Value that you want to convert</param>
    /// <param name="defaultValue">Default value will be used when convert value is null or System.DBNull</param>
    /// <returns>Converted Value</returns>
    public static int ConvertToInteger( object value, int defaultValue )
    {
        try
        {
            if ( value == null || value is DBNull )
            {
                return defaultValue;
            }
            return Convert.ToInt32( value );
        }
        catch
        {
            return defaultValue;
        }
    }
    #endregion

    #region "ConvertToLong"
    /// <summary>
    /// Convert value to Long (Int64) Type
    /// </summary>
    /// <param name="value">Value that you want to convert</param>
    /// <returns>Converted Value</returns>
    public static long ConvertToLong( object value )
    {
        return ConvertToLong( value, 0L );
    }

    /// <summary>
    /// Convert value to Long (Int64) Type
    /// </summary>
    /// <param name="value">Value that you want to convert</param>
    /// <param name="defaultValue">Default value will be used when convert value is null or System.DBNull</param>
    /// <returns>Converted Value</returns>
    public static long ConvertToLong( object value, long defaultValue )
    {
        try
        {
            if ( value == null || value is DBNull )
            {
                return defaultValue;
            }
            return Convert.ToInt64( value );
        }
        catch
        {
            return defaultValue;
        }
    }
    #endregion

    #region "ConvertToDecimal"
    /// <summary>
    /// Convert value to Decimal Type
    /// </summary>
    /// <param name="value">Value that you want to convert</param>
    /// <returns>Converted Value</returns>
    public static decimal ConvertToDecimal( object value )
    {
        return ConvertToDecimal( value, 0M );
    }

    /// <summary>
    /// Convert value to Decimal Type
    /// </summary>
    /// <param name="value">Value that you want to convert</param>
    /// <param name="defaultValue">Default value will be used when convert value is null or System.DBNull</param>
    /// <returns>Converted Value</returns>
    public static decimal ConvertToDecimal( object value, decimal defaultValue )
    {
        try
        {
            if ( value == null || value is DBNull )
            {
                return defaultValue;
            }
            return Convert.ToDecimal( value );
        }
        catch
        {
            return defaultValue;
        }
    }
    #endregion

    #region "ConvertToSingle"
    /// <summary>
    /// Convert value to Single Type
    /// </summary>
    /// <param name="value">Value that you want to convert</param>
    /// <returns>Converted Value</returns>
    public static float ConvertToSingle( object value )
    {
        return ConvertToSingle( value, 0.0f );
    }

    /// <summary>
    /// Convert value to Single Type
    /// </summary>
    /// <param name="value">Value that you want to convert</param>
    /// <param name="defaultValue">Default value will be used when convert value is null or System.DBNull</param>
    /// <returns>Converted Value</returns>
    public static float ConvertToSingle( object value, float defaultValue )
    {
        try
        {
            if ( value == null || value is DBNull )
            {
                return defaultValue;
            }
            return Convert.ToSingle( value );
        }
        catch
        {
            return defaultValue;
        }
    }
    #endregion

    #region "ConvertToDouble"
    /// <summary>
    /// Convert value to Double Type
    /// </summary>
    /// <param name="value">Value that you want to convert</param>
    /// <returns>Converted Value</returns>
    public static double ConvertToDouble( object value )
    {
        return ConvertToDouble( value, 0.0 );
    }

    /// <summary>
    /// Convert value to Double Type
    /// </summary>
    /// <param name="value">Value that you want to convert</param>
    /// <param name="defaultValue">Default value will be used when convert value is null or System.DBNull</param>
    /// <returns>Converted Value</returns>
    public static double ConvertToDouble( object value, double defaultValue )
    {
        try
        {
            if ( value == null || value is DBNull )
            {
                return defaultValue;
            }
            return Convert.ToDouble( value );
        }
        catch
        {
            return defaultValue;
        }
    }
    #endregion

    #region "ConvertToString"
    /// <summary>
    /// Convert value to String Type
    /// </summary>
    /// <param name="value">Value that you want to convert</param>
    /// <param name="defaultValue">Default value will be used when convert value is null or System.DBNull</param>
    /// <returns>Converted Value</returns>
    public static string ConvertToString( object value )
    {
        return ConvertToString( value, String.Empty );
    }

    /// <summary>
    /// Convert value to String Type
    /// </summary>
    /// <param name="value">Value that you want to convert</param>
    /// <param name="defaultValue">Default value will be used when convert value is null or System.DBNull</param>
    /// <returns>Converted Value</returns>
    public static string ConvertToString( object value, string defaultValue )
    {
        try
        {
            if ( value == null || value is DBNull )
            {
                return defaultValue;
            }
            return Convert.ToString( value );
        }
        catch
        {
            return defaultValue;
        }
    }
    #endregion

    #region "ConvertToDateTime"
    /// <summary>
    /// Convert value to DateTime Type
    /// </summary>
    /// <param name="value">Value that you want to convert</param>
    /// <returns>Converted Value</returns>
    public static DateTime ConvertToDateTime( object value )
    {
        return ConvertToDateTime( value, DateTime.MinValue );
    }
    public static DateTime ToDateTimeSQL(object obj)
    {
        IFormatProvider cultureEN = new System.Globalization.CultureInfo("en-US", true);  
        return DateTime.ParseExact(obj.ToString(), "dd/MM/yyyy", cultureEN); ;
    }

    /// <summary>
    /// Convert value to DateTime Type
    /// </summary>
    /// <param name="value">Value that you want to convert</param>
    /// <param name="defaultValue">Default value will be used when convert value is null or System.DBNull</param>
    /// <returns>Converted Value</returns>
    public static DateTime ConvertToDateTime( object value, DateTime defaultValue )
    {
        try
        {
            if ( value == null || value is DBNull )
            {
                return defaultValue;
            }
            return Convert.ToDateTime( value );
        }
        catch
        {
            return defaultValue;
        }
    }
    #endregion

    #region "ConvertToTimeSpan"
    /// <summary>
    /// Convert value to TimeSpan Type
    /// </summary>
    /// <param name="value">Value that you want to convert</param>
    /// <returns>Converted Value</returns>
    public static TimeSpan ConvertToTimeSpan( object value )
    {
        return ConvertToTimeSpan( value, TimeSpan.MinValue );
    }

    /// <summary>
    /// Convert value to TimeSpan Type
    /// </summary>
    /// <param name="value">Value that you want to convert</param>
    /// <param name="defaultValue">Default value will be used when convert value is null or System.DBNull</param>
    /// <returns>Converted Value</returns>
    public static TimeSpan ConvertToTimeSpan( object value, TimeSpan defaultValue )
    {
        try
        {
            if ( value == null || value is DBNull )
            {
                return defaultValue;
            }
            return Convert.ToDateTime( value ).TimeOfDay;
        }
        catch
        {
            return defaultValue;
        }
    }
    #endregion

    #region "ConvertToBool"
    /// <summary>
    /// Convert value to Boolean Type
    /// </summary>
    /// <param name="value">Value that you want to convert</param>
    /// <returns>Converted Value</returns>
    public static Boolean ConvertToBool( object value )
    {
        return ConvertToBool( value, false );
    }

    /// <summary>
    /// Convert value to Boolean Type
    /// </summary>
    /// <param name="value">Value that you want to convert</param>
    /// <param name="defaultValue">Default value will be used when convert value is null or System.DBNull</param>
    /// <returns>Converted Value</returns>
    public static Boolean ConvertToBool( object value, Boolean defaultValue )
    {
        try
        {
            if ( value == null || value is DBNull )
            {
                return defaultValue;
            }
            return Convert.ToBoolean( value );
        }
        catch
        {
            return defaultValue;
        }
    }
    #endregion

    #region "ConvertToGuid"
    /// <summary>
    /// Convert value to Guid Type
    /// </summary>
    /// <param name="value">Value that you want to convert</param>
    /// <returns>Converted Value</returns>
    public static Guid ConvertToGuid( object value )
    {
        return ConvertToGuid( value, Guid.Empty );
    }

    /// <summary>
    /// Convert value to Guid Type
    /// </summary>
    /// <param name="value">Value that you want to convert</param>
    /// <param name="defaultValue">Default value will be used when convert value is null or System.DBNull</param>
    /// <returns>Converted Value</returns>
    public static Guid ConvertToGuid( object value, Guid defaultValue )
    {
        try
        {
            if ( value == null || value is DBNull )
            {
                return defaultValue;
            }
            return Guid.Parse( value.ToString() );
        }
        catch
        {
            return defaultValue;
        }
    }

    //public static decimal? ConvertToDecimal(object priceShipment)
    //{
    //    throw new NotImplementedException();
    //}
    #endregion
}

