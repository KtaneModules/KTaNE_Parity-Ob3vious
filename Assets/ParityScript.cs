using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Rnd = UnityEngine.Random;
using KModkit;
using System.Text.RegularExpressions;

public class ParityScript : MonoBehaviour
{
    public KMAudio Audio;
    public KMBombInfo Bomb;
    public List<MeshRenderer> Leds;
    public List<Transform> ButtonTransformables;
    public List<KMSelectable> Buttons;
    public TextMesh Text;
    public KMBombModule Module;

    private List<Clickable> clickables = new List<Clickable>();
    private Dictionary<char, Permutation> permutations = new Dictionary<char, Permutation>();

    // souv material
    private string _displayedText;

    private bool _solved;

    static int _moduleIdCounter = 1;
    int _moduleID = 0;

    void Awake()
    {
        _moduleID = _moduleIdCounter++;
    }

    void Start()
    {
        clickables.Add(new Switch(Buttons[0], ButtonTransformables[0], Leds[0], this));
        clickables.Add(new Switch(Buttons[1], ButtonTransformables[1], Leds[1], this));
        clickables.Add(new Button(Buttons[2], ButtonTransformables[2], this));

        permutations.Add('A', new Permutation("3412"));
        permutations.Add('B', new Permutation("3421"));
        permutations.Add('C', new Permutation("3124"));
        permutations.Add('D', new Permutation("2143"));
        permutations.Add('E', new Permutation("1432"));
        permutations.Add('F', new Permutation("4321"));
        permutations.Add('G', new Permutation("2431"));
        permutations.Add('H', new Permutation("2134"));
        permutations.Add('I', new Permutation("1342"));
        permutations.Add('J', new Permutation("4213"));
        permutations.Add('K', new Permutation("2413"));
        permutations.Add('L', new Permutation("1423"));
        permutations.Add('M', new Permutation("2314"));
        permutations.Add('N', new Permutation("1234"));
        permutations.Add('P', new Permutation("4132"));
        permutations.Add('Q', new Permutation("3241"));
        permutations.Add('R', new Permutation("1243"));
        permutations.Add('S', new Permutation("4123"));
        permutations.Add('T', new Permutation("4231"));
        permutations.Add('U', new Permutation("3214"));
        permutations.Add('V', new Permutation("1324"));
        permutations.Add('W', new Permutation("4312"));
        permutations.Add('X', new Permutation("3142"));
        permutations.Add('Z', new Permutation("2341"));

        Generate();

        _solved = false;
    }

    void Update()
    {
        foreach (Clickable clickable in clickables)
        {
            clickable.Update(Time.deltaTime);
        }
    }

    public void UpdateModule()
    {
        // check for a solve
        if (_solved)
            return;

        int[] solution = Solution();
        if (solution[0] == ((Switch)clickables[0]).GetState() && solution[1] == ((Switch)clickables[1]).GetState())
        {
            Log("You set the LEDs to the correct states. Solving the module.");
            _solved = true;
            Module.HandlePass();
            _displayedText = Text.text;
            Text.text = "GG";
            return;
        }

        string[] input = new Switch[] { (Switch)clickables[0], (Switch)clickables[1] }.Select(x => new string[] { "off", "on" }[x.GetState()]).ToArray();
        Log("You set the left LED to be {0} and the right LED to be {1}. This is incorrect. Regenerating the module.", input[0], input[1]);
        Module.HandleStrike();
        Generate();
    }

    private void Generate()
    {
        Text.text = "" + "ABCDEFGHIJKLMNPQRSTUVWXZ"[Rnd.Range(0, 24)] + Rnd.Range(0, 10);
        string[] solution = Solution().Select(x => new string[] { "off", "on" }[x]).ToArray();
        Log("The displayed characters are {0}. Expecting the left LED to be {1} and the right LED to be {2}.", Text.text, solution[0], solution[1]);
    }

    private int[] Solution()
    {
        Permutation main = permutations[Text.text[0]];
        bool parity1 = ((Bomb.GetSerialNumber()[2] + Text.text[1]) % 2 == 0);
        int switch1 = permutations[Bomb.GetSerialNumber()[3]].CompareTo(main) == parity1 ? 1 : 0;
        bool parity2 = ((Bomb.GetSerialNumber()[5] + Text.text[1]) % 2 == 0);
        int switch2 = permutations[Bomb.GetSerialNumber()[4]].CompareTo(main) == parity2 ? 0 : 1;

        return new int[] { switch1, switch2 };
    }

    private void Log(string message, params object[] args)
    {
        string moduleName = "Parity";
        Debug.LogFormat("[" + moduleName + " #" + _moduleID + "] " + message, args);
    }

#pragma warning disable 414
    private string TwitchHelpMessage = "'!{0} submit off on' to submit the module with the left LED off and the right LED on.";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        yield return null;
        command = command.ToLowerInvariant();
        if (new Regex(@"^submit(\s(on|off)){2}$").IsMatch(command))
        {
            int i = 0;
            foreach (Match m in new Regex(@"(on|off)").Matches(command))
            {
                if ((m.ToString() == "on" ? 1 : 0) != ((Switch)clickables[i]).GetState())
                {
                    Buttons[i].OnInteract();
                    yield return new WaitForSeconds(0.1f);
                }
                i++;
            }
            Buttons[2].OnInteract();
            yield return new WaitForSeconds(0.1f);
            Buttons[2].OnInteractEnded();
        }
        else
        {
            yield return "sendtochaterror Invalid command.";
            yield break;
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        yield return true;
        int i = 0;
        foreach (int n in Solution())
        {
            if (n != ((Switch)clickables[i]).GetState())
            {
                Buttons[i].OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
            i++;
        }
        Buttons[2].OnInteract();
        yield return new WaitForSeconds(0.1f);
        Buttons[2].OnInteractEnded();
    }
}
