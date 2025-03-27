
namespace DotHttpTest.Runner
{
    /// <summary>
    /// Parses stage metadata attributes from .http files and creates Stage objects.
    /// Stage objects are declared on a request but may apply to many request objects
    /// (They apply to all request objects until the next stage is declared)
    /// </summary>
    public class StageParser
    {
        /// <summary>
        /// # @stage ramp-up 	duration:30s target:100
        /// # @stage soak 		duration:2h  target:100
        /// # @stage ramp-down 	duration:5m  target:0
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static StageAttributes Parse(string line)
        {
            var stage = new StageAttributes();
            var items = line.Split(new char[] { ' ' , '\t'}, StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries);
            stage.Name = items[0];

            foreach(var propertyPairString in items.Skip(1))
            {
                ParsePropertyPairString(stage, propertyPairString);
            }

            return stage;
        }

        private static bool TryParseDuration(string line, out TimeSpan duration)
        {
            duration = TimeSpan.Zero;
            var suffixes = new char[] { 'd', 'h', 'm', 's'};
            var digits = new StringBuilder();
            foreach(var digitOrCharacter in line)
            {
                if(suffixes.Contains(digitOrCharacter))
                {
                    if(int.TryParse(digits.ToString(), out int number))
                    {
                        switch(digitOrCharacter)
                        {
                            case 'd':
                                duration += TimeSpan.FromDays(number);
                                break;
                            case 'h':
                                duration += TimeSpan.FromHours(number);
                                break;
                            case 'm':
                                duration += TimeSpan.FromMinutes(number);
                                break;
                            case 's':
                                duration += TimeSpan.FromSeconds(number);
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }
                    digits.Clear();
                }
                else if(Char.IsDigit(digitOrCharacter))
                {
                    digits.Append(digitOrCharacter);   
                }
            }

            return duration != TimeSpan.Zero;
        }

        private static void ParsePropertyPairString(StageAttributes stage, string propertyPairString)
        {
            var pair = propertyPairString.Split(':');
            if(pair.Length != 2)
            {
                throw new HttpParsingException($"Invalid property '{propertyPairString}' - expected : to separate key and value pair.");
            }
            switch(pair[0])
            {
                case "duration":
                    if (TryParseDuration(pair[1], out TimeSpan duration))
                    {
                        stage.Duration = duration;
                    }
                    else
                    {
                        throw new HttpParsingException($"Error parsing property 'duration'. For one-shot requests remove the duration property.");
                    }
                    break;

                case "count":
                    if (!int.TryParse(pair[1], out var requestCount))
                    {
                        throw new HttpParsingException($"Invalid property '{pair[1]}' - expected integer value for property 'count'");
                    }
                    stage.Iterations = requestCount;
                    break;

                case "target":
                    if (!int.TryParse(pair[1], out var target))
                    {
                        throw new HttpParsingException($"Invalid property '{pair[1]}' - expected integer value for property 'target'");
                    }
                    stage.Target = target;
                    break;
            }
        }
    }
}
