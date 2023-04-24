using System;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Intent;

namespace AzureintentRecognizer
{
    class Program
    {
        // This example requires environment variables named "SPEECH_KEY" and "SPEECH_REGION"
        static string speechKey = "df9e0ccde1374f9eaab1d48a94624769";
        static string speechRegion = "southeastasia";

        static void Main(string[] args)
        {
            IntentPatternMatchingWithMicrophoneAsync().Wait();
        }

        private static async Task IntentPatternMatchingWithMicrophoneAsync()
        {
            var config = SpeechConfig.FromSubscription(speechKey, speechRegion);
            using (var intentRecognizer = new IntentRecognizer(config))
            {
                intentRecognizer.AddIntent("Take me to floor {floorName}.", "ChangeFloors");
                intentRecognizer.AddIntent("Go to floor {floorName}.", "ChangeFloors");
                intentRecognizer.AddIntent("{action} the door.", "OpenCloseDoor");

                Console.WriteLine("Say something...");

                var result = await intentRecognizer.RecognizeOnceAsync();

                string floorName;
                switch (result.Reason)
                {
                    case ResultReason.RecognizedSpeech:
                        Console.WriteLine($"RECOGNIZED: Text= {result.Text}");
                        Console.WriteLine($"    Intent not recognized.");
                        break;
                    case ResultReason.RecognizedIntent:
                        Console.WriteLine($"RECOGNIZED: Text= {result.Text}");
                        Console.WriteLine($"       Intent Id= {result.IntentId}.");
                        var entities = result.Entities;
                        if (entities.TryGetValue("floorName", out floorName))
                        {
                            Console.WriteLine($"       FloorName= {floorName}");
                        }

                        if (entities.TryGetValue("action", out floorName))
                        {
                            Console.WriteLine($"       Action= {floorName}");
                        }

                        break;
                    case ResultReason.NoMatch:
                        {
                            Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                            var noMatch = NoMatchDetails.FromResult(result);
                            switch (noMatch.Reason)
                            {
                                case NoMatchReason.NotRecognized:
                                    Console.WriteLine($"NOMATCH: Speech was detected, but not recognized.");
                                    break;
                                case NoMatchReason.InitialSilenceTimeout:
                                    Console.WriteLine($"NOMATCH: The start of the audio stream contains only silence, and the service timed out waiting for speech.");
                                    break;
                                case NoMatchReason.InitialBabbleTimeout:
                                    Console.WriteLine($"NOMATCH: The start of the audio stream contains only noise, and the service timed out waiting for speech.");
                                    break;
                                case NoMatchReason.KeywordNotRecognized:
                                    Console.WriteLine($"NOMATCH: Keyword not recognized");
                                    break;
                            }
                            break;
                        }
                    case ResultReason.Canceled:
                        {
                            var cancellation = CancellationDetails.FromResult(result);
                            Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                            if (cancellation.Reason == CancellationReason.Error)
                            {
                                Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                                Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                                Console.WriteLine($"CANCELED: Did you set the speech resource key and region values?");
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
        }
    }
}