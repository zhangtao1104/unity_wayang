namespace agora
{
    namespace unity
    {
        public enum LOG_LEVEL_TYPE
        {
            ALL = 0,
            INFO = 1,
            WARNING = 2,
            ERROR = 3,
            NONE = 4, 
        }

        public interface ILogHelper
        {
            LOG_LEVEL_TYPE LogLevel 
            {
                get;
                set;
            }

            string Title
            {
                get;
                set;
            }

            string FormatInfo
            {
                get;
                set;
            }

            string FormatWarning
            {
                get;
                set;
            }

            string FormatError
            {
                get;
                set;
            }

            void Info (string tag, string logMessage);
            void Warning (string tag, string logMessage);
            void Error (string tag, string logMessage);
        }
    }
}
