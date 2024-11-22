using Newtonsoft.Json;
using System.Web;




bool isPlaying = true;
bool isEmpty;
bool success;
bool isNumber;
int answer;

int counterRight = 0;
int counterWrong = 0;
Random random = new Random();


string filePath = @"./questions.txt";

List<QuizItem> quizItems = new List<QuizItem>();

try
{
    string json = File.ReadAllText(filePath);
    quizItems = JsonConvert.DeserializeObject<List<QuizItem>>(json)!;
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Error reading file: " + ex.Message);
    return;
}




Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("Welcome to the Trivia!\n" +
    "I'm gonna ask you questions and you'll get 4 answer examples. \n" +
    "You answer by pressing the corresponding number on your keyboard (1, 2, 3, 4)\n" +
    "If you answer correct 10 times you win. If you answer wrong 10 times you loose.\n" +
    "Good Luck!\n");



while (isPlaying)
{
    Console.ForegroundColor= ConsoleColor.White;

    int rndIndex = random.Next(0, quizItems.Count);

    QuizItem randomQuestion = quizItems[rndIndex];

    string decodedQuestion = HttpUtility.HtmlDecode(randomQuestion.Question);
    string decodedAnswer = HttpUtility.HtmlDecode(randomQuestion.CorrectAnswer);

    List<string> decodedIncorrect = new List<string>();
    foreach (string item in randomQuestion.IncorrectAnswers) 
    {
        decodedIncorrect.Add(HttpUtility.HtmlDecode(item));
    }


    var (shuffledAnswers, correctAnswerIndex) = randomQuestion.ShuffledAnswers(decodedAnswer, decodedIncorrect);


    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("\n" + decodedQuestion + "\n");

    for (int i = 0; i < shuffledAnswers.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {shuffledAnswers[i]}");
    }
    Console.WriteLine();

    string str = Console.ReadLine()!;
    isEmpty = string.IsNullOrEmpty(str);
    success = int.TryParse(str, out answer);
    isNumber = str.All(char.IsDigit);

    bool valid = false;
    while (!valid)
    {

        if (!success || !isNumber || answer < 1 || answer > shuffledAnswers.Count)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nInvalid answer! You have to answer with the corresponding number.\n");

            str = Console.ReadLine()!;
            isEmpty = string.IsNullOrEmpty(str);
            success = int.TryParse(str, out answer);
            isNumber = str.All(char.IsDigit);
        }
        else
        {
            valid = true;
        }
    }


    if (answer == correctAnswerIndex + 1)
    {
        Console.Clear();
        counterRight++;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nWOW! You got it right!\n");
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("Correct guesses:\t" + counterRight);
        Console.ForegroundColor= ConsoleColor.Red;
        Console.WriteLine("Wrong guesses:\t\t" + counterWrong);
    }
    else
    {
        Console.Clear();
        counterWrong++;

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\nWrong! \nThe right answer is: \t{decodedAnswer}\n");
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("Correct guesses:\t" + counterRight);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Wrong guesses:\t\t" + counterWrong);
    }



    if (counterWrong == 10)
    {
        Console.Clear();
        Console.WriteLine("\nHAHAHA! You loose!\n");

        bool continuePlaying = false;
        while (!continuePlaying)
        {
            Console.ForegroundColor= ConsoleColor.White;
            Console.Write("Would you like to try again?\t");
            string willContinue = Console.ReadLine()!.ToUpper();

            isEmpty = string.IsNullOrEmpty(willContinue);
            isNumber = willContinue.All(char.IsDigit);

            if (isEmpty || isNumber || willContinue != "Y" && willContinue != "N")
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Incorrect input. Press Y or N.\n");
            }
            else if (willContinue == "Y")
            {
                Console.Clear();
                Console.WriteLine("Let's go!\n");
                counterRight = 0;
                counterWrong = 0;
                continuePlaying = true;
            }
            else
            {
                continuePlaying = true;
                isPlaying = false;
            }
        }
    }
    else if (counterRight == 10)
    {
        Console.Clear();
        Console.WriteLine("Congrats! You won! Good job.\n");

        bool continuePlaying = false;
        while (!continuePlaying)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Would you like to try again?\t");
            string willContinue = Console.ReadLine()!.ToUpper();

            isEmpty = string.IsNullOrEmpty(willContinue);
            isNumber = willContinue.All(char.IsDigit);

            if (isEmpty || isNumber || willContinue != "Y" && willContinue != "N")
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Incorrect input. Press Y or N.\n");
            }
            else if (willContinue == "Y")
            {
                Console.Clear();
                Console.WriteLine("Let's go!\n");
                counterRight = 0;
                counterWrong = 0;
                continuePlaying = true;
            }
            else
            {
                continuePlaying = true;
                isPlaying = false;
            }
        }
    }
}

public class QuizItem
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("question")]
    public string Question { get; set; }

    [JsonProperty("correct_answer")]
    public string CorrectAnswer { get; set; }

    [JsonProperty("incorrect_answers")]
    public List<string> IncorrectAnswers { get; set; }


    public QuizItem(string CorrectAnswer, List<string> IncorrectAnswers)
    {
        this.CorrectAnswer = CorrectAnswer;
        this.IncorrectAnswers = IncorrectAnswers ?? new List<string>();
    }

    public (List<string> shuffledAnswers, int correctAnswerIndex) ShuffledAnswers(string decodedAnswer, List<string> decodedIncorrect)
    {
        Random rnd = new Random();

        List<string> shuffledAnswers = new List<string>(decodedIncorrect);
        shuffledAnswers.Add(decodedAnswer);

        int i = shuffledAnswers.Count;

        while ( i > 1)
        {
            i--;
            int j = rnd.Next(i +1);
            var temp = shuffledAnswers[j];
            shuffledAnswers[j] = shuffledAnswers[i];
            shuffledAnswers[i] = temp;
        }

        int correctAnswerIndex = shuffledAnswers.IndexOf(decodedAnswer);

        return (shuffledAnswers, correctAnswerIndex);
    }

}

