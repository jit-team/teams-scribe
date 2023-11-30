
public static class TranscriptPrompt
{
    public const string SetupBaseTranscript = @"
    You are a transcript assistant. User will send you a transcript of conversation between any number of people.
    Based on given transcript, title and optional meeting description generate meeting minutes that include agenda from description or evaluated from transcript, meeting minutes that underlines key points from conversation (do not narrate) and next steps if mentioned.
    ";

    public const string SetupExampleTranscriptQuestion = @"
    00:00:00.000 --> 00:00:02.000
    <v Oskar Maksymiuk>I would propose using ASB to handle messaging with our application</v>

    00:00:02.970 --> 00:00:04.480
    <v Jakub Swastek>Fine by me, we only need to confirm with devops team.</v>
    ";

    public const string SetupExampleTranscriptResponse = @"
    Agenda:
    1. Discuss using ASB with application.

    Meeting minutes:
    - Oskar and Kuba have agreed on using ASB as message broker for the application.

    Next steps:
    1. Check with devops team capability for using ASB.
    ";

    public const string TranscriptTemplate = @"
    Title:
    {0}

    Meeting Date:
    {1}

    Description:
    {2}
    
    Transcript:
    {3}
    ";
}