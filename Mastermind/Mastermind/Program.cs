using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastermind_Project
{
    // the historyQueue class
    public class Queue
    {
        public int back = -1;                   // the pointer of the queue is set to -1 
        public int[] data = new int[1000];        // here the history-array gets created. size is 1000 but could be any
    }

    class Program
    {
        /*
         * The porpouse of this program is to emulate the game Mastermind with the implementation of a Queue. 
         * After the secret code is created, the user is asked to guess its four digits. 
         */

        static void Main(string[] args)
        {
            Queue historyQueue = new Queue();     // the history-queue is here created

            int[] secretCode;                       // the secret code is declared

            int attempts = 0;                       // memorises how many set of guesses have been done 

            int blackPegs = 0;                      // stores the number of total black pegs

            uint numberOfPegs = 0;

            string inputString = "yes";             //the condition that makes the do-while statement execute

            Console.Write("Welcome to MasterMind! ");

            secretCode = GenerateSecretCombo(ref numberOfPegs);             //calls the method that creates the secret code and stores it in int [] secretCode

            //this do-while loops is the core of the program execution
            do
            {
                attempts++;

                int pointer = 0;                // the pointer used to iterate through the history-queue

                if (attempts > 1)
                {
                    Console.WriteLine("\n***PREVIOUS ATTEMPTS***\n");
                }

                for (int j = 0; j < attempts - 1; j++)
                {
                    //this evaluates ALL the guesses located in the history-queue
                    blackPegs = Evaluate(historyQueue, secretCode, ref pointer, numberOfPegs);

                    Console.WriteLine();
                }

                //if the secret combo has not been cracked
                if (!(blackPegs == numberOfPegs))
                {
                    AskCombination(historyQueue, numberOfPegs, attempts);
                }

                //if the combo has been broken the game secret code gets reset and the history is reset 
                else
                {
                    Console.WriteLine("Great job, you just won! Would you like to play again? Type 'yes' to play again.");

                    //any answer different from yes will kill the program
                    inputString = Console.ReadLine();

                    if (inputString == "yes")
                    {

                        attempts = 0;

                        historyQueue = new Queue();     // the history-queue is here created

                        blackPegs = 0;

                        numberOfPegs = 0;

                        secretCode = GenerateSecretCombo(ref numberOfPegs);

                    }

                }

            } while (inputString == "yes");
        }

        // AskCombination asks the combination to the player and loops according to the number of pegs you wanted to play with
        static void AskCombination(Queue historyQueue, uint numberOfPegs, int attempts)
        {
            uint guess;

            if (attempts > 1)
                Console.WriteLine("Retry, you will be luckier! Make sure the number is in range.");

            // the following for-loops take add the user guesses to the history-queue
            for (int i = 0; i < numberOfPegs; i++)
            {
                //this while-statement avoid the user to input anything that is not an uint
                while (!uint.TryParse(Console.ReadLine(), out guess))
                {
                    Console.WriteLine("Invalid input.");
                }

                //this adds every guess in the history-queue
                Add(historyQueue, (int)guess);
            }

        }


        //the following method will generate the secret combination based on the user inputs
        static int[] GenerateSecretCombo(ref uint numberOfPegs)
        {
            Random randNum = new Random();                  // this will be used to generate the randomn digits of the secret combination

            uint colours = 0;                            //stores the number of colours the user wants to play with

            Console.WriteLine("Now, how many colours would you like to play with?");

            while (!uint.TryParse(Console.ReadLine(), out colours))
            {
                Console.WriteLine("Invalid input.");
            }

            Console.WriteLine("...and how many pegs?");

            while (!uint.TryParse(Console.ReadLine(), out numberOfPegs))
            {
                Console.WriteLine("Invalid input.");
            }

            int[] secretCode = new int[numberOfPegs];  //here the secret code is initiated

            //the following for-loop creates the secret code by adding random values from 1 to 6 into a 4-integer array
            for (int i = 0; i < numberOfPegs; i++)
            {
                secretCode[i] = randNum.Next(1, (int)colours);
            }

            Console.WriteLine("Thank you, the secret combination is now created. Can you guess it? Please input {0} colours (technically numbers), one at time, ranging from 1 to {1}", numberOfPegs, colours);

            return secretCode;      //returns the secretCode array
        }


        /*
         * The "Evaluate method" compares the user guess with the secret combination
         * to find out the number of black pegs and white pegs
         */
        static int Evaluate(Queue guessesHistory, int[] secretCode, ref int pointer, uint numberOfPegs)
        {
            int blackPegs = 0;                              //stores the number of black pegs
            int whitePegs = 0;                              //stores the number of white pegs

            int userGuessesLeft = 0;                        //these two variables are used for 
            int secretCodeLeft = 0;                         // the evaluation of white pegs

            int[] userGuesses = new int[numberOfPegs];
            int[] userGuessesBucket = new int[numberOfPegs];
            int[] secretCodeBucket = new int[numberOfPegs];

            // The user guesses are copied into a temporary array.
            // The pointer gets increased everytime a new entry is added in the history-queue.
            // That helps to keep track of the guesses that need to be evaluated
            for (int i = 0; i < numberOfPegs; i++)
            {
                userGuesses[i] = guessesHistory.data[pointer];
                pointer++;
            }

            //The following for-loop, finds out how many black pegs there are.
            for (int i = 0; i < numberOfPegs; i++)
            {
                //if the index(position) and the integer(colour) are equal, the blackPegs variable increases
                if (userGuesses[i] == secretCode[i])
                {
                    blackPegs++;
                }

                //if the guess is not black, it gets stored to another array to be analysed later on
                else
                {
                    userGuessesBucket[userGuessesLeft] = userGuesses[i];
                    secretCodeBucket[secretCodeLeft] = secretCode[i];

                    userGuessesLeft++;
                    secretCodeLeft++;
                }
            }

            //this for-loop works out the number of white pegs
            for (int i = 0; i < userGuessesLeft; i++)
            {
                for (int j = 0; j < secretCodeLeft; j++)
                {
                    if (userGuessesBucket[i] == secretCodeBucket[j])
                    {
                        //if any number in the secretCodeBucket is equal to userGuessesLeft[i], the white pegs increase
                        whitePegs++;
                        //the secret code left dwindles
                        secretCodeLeft--;
                        // the last integer shuffles with the integer just analyzed
                        secretCodeBucket[j] = secretCodeBucket[secretCodeLeft];
                        //the loops breaks because the white peg increased
                        break;
                    }
                }
            }

            PrintHistory(pointer, numberOfPegs, userGuesses, blackPegs, whitePegs);

            //return the number of black pegs 
            return blackPegs;
        }

        //this prints in every step, all the guesses done so far
        public static void PrintHistory(int pointer, uint numberOfPegs, int[] userGuesses, int blackPegs, int whitePegs)
        {
            //Print out the outcomes
            Console.Write("Attempt {0}: [ ", pointer / numberOfPegs);

            for (int i = 0; i < numberOfPegs; i++)
            {
                Console.Write("{0} ", userGuesses[i]);
            }
            Console.WriteLine("] {0} black peg/s and {1} white peg/s", blackPegs, whitePegs);
        }

        //a basic method to add the guesses in the history-queue
        static void Add(Queue historyQueue, int guess)
        {
            if (historyQueue.back == 999)
            {
                System.Console.WriteLine("The history-queue is full. You excedeed the maximum number of guesses. The game will now end.");
                System.Environment.Exit(1);

            }

            //the else statement inserts the guess into the history-queue
            else
            {
                historyQueue.back = historyQueue.back + 1;
                historyQueue.data[historyQueue.back] = guess;
            }
        }
    }
}

