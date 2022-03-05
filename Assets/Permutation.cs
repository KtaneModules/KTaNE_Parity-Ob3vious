public class Permutation {
    private string _permutation;

    public Permutation(string permutation)
    {
        _permutation = permutation;
    }

    public bool CompareTo(Permutation p2)
    {
        int steps = 0;
        string temp = _permutation;

        for (int i = 0; i < temp.Length; i++)
        {
            int swapIdx = temp.IndexOf(p2._permutation[i]);
            if (swapIdx == i)
                continue;

            steps++;
            string newTemp = "";
            for (int j = 0; j < temp.Length; j++)
            {
                if (j == i)
                    newTemp += temp[swapIdx];
                else if (j == swapIdx)
                    newTemp += temp[i];
                else
                    newTemp += temp[j];
            }

            temp = newTemp;
        }

        return steps % 2 == 0;
    }

    public override string ToString()
    {
        return _permutation;
    }
}
