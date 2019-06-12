using System;

namespace Airport.BL
{
	public static class Helper
	{
		public static Random rnd = new Random();
		public static int GetRandom(int start, int end)
		{
			return rnd.Next(start, end);
		}
	}
}
