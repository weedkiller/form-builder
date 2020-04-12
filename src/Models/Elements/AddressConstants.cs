﻿
using System.Text.RegularExpressions;

namespace form_builder.Models.Elements
{
    public class AddressConstants
    {
                public const string POSTCODE = "postcode";

                public const string ADDRESS_SELECT = "address";

                public static Regex UPRN_REGEX = new Regex(@"^[0-9]{12}$");

                public static Regex POSTCODE_REGEX = new Regex(@"^([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9][A-Za-z]?))))\s?[0-9][A-Za-z]{2})$");
                
                // TODO: Think this should be renamed "Custom" rather than "Stockport" and value moved to config
                public static Regex STOCKPORT_POSTCODE_REGEX = new Regex(@"^(sK|Sk|SK|sk|M|m)[0-9][0-9A-Za-z]?\s?[0-9][A-Za-z]{2}");

    }
}