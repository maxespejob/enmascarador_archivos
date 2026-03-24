using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterchangeFilesMaskingApp
{
    internal class DataElements
    {
        public enum ElementType
        {
            Fixed,
            LLVar,
            LLLVar,
        }

        public struct Element
        {
            public string Name;
            public ElementType Type;
            public uint? Length;
        }

        public static readonly Dictionary<uint, Element> ElementDict = new Dictionary<uint, Element>
            {
                {
                    1,
                    new Element
                    {
                        Name = "Bitmap Secondary",
                        Type = ElementType.Fixed,
                        Length = 8,
                    }
                },
                {
                    2,
                    new Element
                    {
                        Name = "PAN",
                        Type = ElementType.LLVar,
                        Length = 2,
                    }
                },
                {
                    3,
                    new Element
                    {
                        Name = "Processing Code",
                        Type = ElementType.Fixed,
                        Length = 6,
                    }
                },
                {
                    4,
                    new Element
                    {
                        Name = "Amount, Transaction",
                        Type = ElementType.Fixed,
                        Length = 12,
                    }
                },
                {
                    5,
                    new Element
                    {
                        Name = "Amount, Reconciliation",
                        Type = ElementType.Fixed,
                        Length = 12,
                    }
                },
                {
                    6,
                    new Element
                    {
                        Name = "Amount, Cardholder Billing",
                        Type = ElementType.Fixed,
                        Length = 12,
                    }
                },
                {
                    9,
                    new Element
                    {
                        Name = "Conversion Rate, Reconciliation",
                        Type = ElementType.Fixed,
                        Length = 8,
                    }
                },
                {
                    10,
                    new Element
                    {
                        Name = "Conversion Rate, Cardholder Billing",
                        Type = ElementType.Fixed,
                        Length = 8,
                    }
                },
                {
                    12,
                    new Element
                    {
                        Name = "Date/Time Local Transaction",
                        Type = ElementType.Fixed,
                        Length = 12,
                    }
                },
                {
                    14,
                    new Element
                    {
                        Name = "Expiration Date",
                        Type = ElementType.Fixed,
                        Length = 4,
                    }
                },
                {
                    22,
                    new Element
                    {
                        Name = "Point of Service Data Code",
                        Type = ElementType.Fixed,
                        Length = 12,
                    }
                },
                {
                    23,
                    new Element
                    {
                        Name = "Card Sequence Number",
                        Type = ElementType.Fixed,
                        Length = 3,
                    }
                },
                {
                    24,
                    new Element
                    {
                        Name = "Function Code",
                        Type = ElementType.Fixed,
                        Length = 3
                    }
                },
                {
                    25,
                    new Element
                    {
                        Name = "Message Reason Code",
                        Type = ElementType.Fixed,
                        Length = 4,
                    }
                },
                {
                    26,
                    new Element
                    {
                        Name = "Card Acceptor Business Code",
                        Type = ElementType.Fixed,
                        Length = 4,
                    }
                },
                {
                    30,
                    new Element
                    {
                        Name = "Amounts, Original",
                        Type = ElementType.Fixed,
                        Length = 24,
                    }
                },
                {
                    31,
                    new Element
                    {
                        Name = "Acquirer Reference Data",
                        Type = ElementType.LLVar,
                        Length = 2,
                    }
                },
                {
                    32,
                    new Element
                    {
                        Name = "Acquiring Institution ID Code",
                        Type = ElementType.LLVar,
                        Length = 2,
                    }
                },
                {
                    33,
                    new Element
                    {
                        Name = "Forwarding Institution ID Code",
                        Type = ElementType.LLVar,
                        Length = 2,
                    }
                },
                {
                    37,
                    new Element
                    {
                        Name = "Retrieval Reference Number",
                        Type = ElementType.Fixed,
                        Length = 12,
                    }
                },
                {
                    38,
                    new Element
                    {
                        Name = "Approval Code",
                        Type = ElementType.Fixed,
                        Length = 6
                    }
                },
                {
                    40,
                    new Element
                    {
                        Name = "Service Code",
                        Type = ElementType.Fixed,
                        Length = 3
                    }
                },
                {
                    41,
                    new Element
                    {
                        Name = "Card Acceptor Terminal ID",
                        Type = ElementType.Fixed,
                        Length = 8,
                    }
                },
                {
                    42,
                    new Element
                    {
                        Name = "Card Acceptor ID",
                        Type = ElementType.Fixed,
                        Length = 15,
                    }
                },
                {
                    43,
                    new Element
                    {
                        Name = "Card Acceptor Name/Location",
                        Type = ElementType.LLVar,
                        Length = 2,
                    }
                },
                {
                    48,
                    new Element
                    {
                        Name = "Additional Data",
                        Type = ElementType.LLLVar,
                        Length = 3,
                    }
                },
                {
                    49,
                    new Element
                    {
                        Name = "Currency Code, Transaction",
                        Type = ElementType.Fixed,
                        Length = 3,
                    }
                },
                {
                    50,
                    new Element
                    {
                        Name = "Currency Code, Reconciliation",
                        Type = ElementType.Fixed,
                        Length = 3,
                    }
                },
                {
                    51,
                    new Element
                    {
                        Name = "Currency Code, Cardholder Billing",
                        Type = ElementType.Fixed,
                        Length = 3,
                    }
                },
                {
                    54,
                    new Element
                    {
                        Name = "Amounts, Additional",
                        Type = ElementType.LLLVar,
                        Length = 3,
                    }
                },
                {
                    55,
                    new Element
                    {
                        Name = "ICC System Related Data",
                        Type = ElementType.LLLVar,
                        Length = 3,
                    }
                },
                {
                    62,
                    new Element
                    {
                        Name = "Additional Data 2",
                        Type = ElementType.LLLVar,
                        Length = 3,
                    }
                },
                {
                    63,
                    new Element
                    {
                        Name = "Transaction Lifecycle ID",
                        Type = ElementType.LLLVar,
                        Length = 3,
                    }
                },
                {
                    71,
                    new Element
                    {
                        Name = "Message Number",
                        Type = ElementType.Fixed,
                        Length = 8,
                    }
                },
                {
                    72,
                    new Element
                    {
                        Name = "Data Record",
                        Type = ElementType.LLLVar,
                        Length = 3
                    }
                },
                {
                    73,
                    new Element
                    {
                        Name = "Date, Action",
                        Type = ElementType.Fixed,
                        Length = 6
                    }
                },
                {
                    93,
                    new Element
                    {
                        Name = "Transaction Destination Institution ID",
                        Type = ElementType.LLVar,
                        Length = 2,
                    }
                },
                {
                    94,
                    new Element
                    {
                        Name = "Transaction Originator Institution ID",
                        Type = ElementType.LLVar,
                        Length = 2,
                    }
                },
                {
                    95,
                    new Element
                    {
                        Name = "Card Issuer Reference Data",
                        Type = ElementType.LLVar,
                        Length = 2,
                    }
                },
                {
                    100,
                    new Element
                    {
                        Name = "Receiving Institution ID",
                        Type = ElementType.LLVar,
                        Length = 2,
                    }
                },
                {
                    105,
                    new Element
                    {
                        Name = "Additional Data 6",
                        Type = ElementType.LLLVar,
                        Length = 3,
                    }
                },
                {
                    111,
                    new Element
                    {
                        Name = "Amount, Currency Conversion Assignment",
                        Type = ElementType.LLLVar,
                        Length = 3,
                    }
                },
                {
                    123,
                    new Element
                    {
                        Name = "Additional Data 3",
                        Type = ElementType.LLLVar,
                        Length = 3,
                    }
                },
                {
                    124,
                    new Element
                    {
                        Name = "Additional Data 4",
                        Type = ElementType.LLLVar,
                        Length = 3,
                    }
                },
                {
                    125,
                    new Element
                    {
                        Name = "Additional Data 5",
                        Type = ElementType.LLLVar,
                        Length = 3,
                    }
                },
                {
                    127,
                    new Element
                    {
                        Name = "Network Data",
                        Type = ElementType.LLLVar,
                        Length = 3,
                    }
                },
            };
    }
}
