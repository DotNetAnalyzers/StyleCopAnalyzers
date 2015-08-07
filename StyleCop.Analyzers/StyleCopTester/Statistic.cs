namespace StyleCopTester
{
    internal struct Statistic
    {
        public Statistic(int numberOf1, int numberOf2, int numberOf3)
        {
            this.NumberofNodes = numberOf1;
            this.NumberOfTokens = numberOf2;
            this.NumberOfTrivia = numberOf3;
        }

        public int NumberofNodes { get; set; }

        public int NumberOfTokens { get; set; }

        public int NumberOfTrivia { get; set; }

        public static Statistic operator +(Statistic statistic1, Statistic statistic2)
        {
            return new Statistic(statistic1.NumberofNodes + statistic2.NumberofNodes,
                    statistic1.NumberOfTokens + statistic2.NumberOfTokens,
                    statistic1.NumberOfTrivia + statistic2.NumberOfTrivia);
        }
    }
}
