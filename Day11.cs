using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AdventofCode2025
{
	internal static class Day11
	{
		public class Device
		{
			public readonly string ID;
			public readonly string[] Outputs;

			public int pings = 0;
			public int pingsDAC = 0;
			public int pingsFFT = 0;
			public int pingsYOU = 0;

			public Device(string line)
			{
				string[] parts = line.Split(':');
				ID = parts[0];
				Outputs = parts[1].Split(' ').Where(s => !string.IsNullOrEmpty(s)).ToArray();
			}

			public void Pulse(Dictionary<string, Device> allDevices)
			{
				pings++;
				foreach (string o in Outputs)
				{
					allDevices[o].Pulse(allDevices);
				}
			}

			public void Pulse(Dictionary<string, Device> allDevices, bool visitDAC, bool visitFFT)
			{
				if (ID == "you")
				{
					var y = allDevices["dac"];
					pingsYOU++;
					if (visitFFT)
					{
						pingsFFT++;
					}
					if (visitDAC)
					{
						pingsDAC++;
					}
					return;
				}

				if (this.ID == "dac")
				{
					var y = allDevices["you"];
					pingsDAC++;
					visitDAC = true;
					if (visitFFT)
					{
						pingsFFT++;
					}
					return;
				}

				if (this.ID == "fft")
				{
					if (visitFFT)
					{
						pings++;
						return;
					}
					if (visitDAC)
					{
						pingsDAC++;
						return;
					}
					pingsFFT++;
					visitFFT = true;
				}

				if (this.ID == "out" && visitFFT && visitDAC)
				{
					pings++;
					return;
				}

				foreach (string o in Outputs)
				{
					/*if (o == "you")
					{
						if (!visitDAC)
						{
							continue;
						}
						if (!visitFFT)
						{
							continue;
						}
					}*/

					allDevices[o].Pulse(allDevices, visitDAC, visitFFT);
				}

				/*if (this.ID == "fft")
					visitFFT = true;
				if(this.ID == "out")
				{
					;
					if(visitDAC && visitFFT)
						pings++;
				}
				foreach (string o in Outputs)
				{
					allDevices[o].Pulse(allDevices, visitDAC, visitFFT);
				}*/
			}
		}

		internal static long Part1(string input)
		{
			string[] lines = input.Split('\n');
			long result = 0l;
			Dictionary<string,Device> allDevices = new Dictionary<string, Device>();
			foreach (string line in lines)
			{
				Device dev = new Device(line);
				allDevices.Add(dev.ID,dev);
			}
			allDevices.Add("out", new Device("out:"));
			allDevices["you"].Pulse(allDevices);
			return allDevices["out"].pings;
		}

		internal static long Part2(string input)
		{
			string[] lines = input.Split('\n');
			long result = 0l;
			Dictionary<string, Device> allDevices = new Dictionary<string, Device>();
			foreach (string line in lines)
			{
				Device dev = new Device(line);
				allDevices.Add(dev.ID, dev);
			}
			allDevices.Add("out", new Device("out:"));
			allDevices["fft"].Pulse(allDevices, false, false);

			//Console.WriteLine($"You: {allDevices["you"].pings}\nYouD {allDevices["you"].pingsDAC}\nOut: {allDevices["out"].pings}");
			var f = allDevices["fft"];
			var d = allDevices["dac"];
			var y = allDevices["you"];

			Console.WriteLine($"FFT -> DAC: {d.pingsFFT}");

			f.pingsDAC = f.pingsFFT = f.pingsYOU = f.pings = 0;
			d.pingsDAC = d.pingsFFT = d.pingsYOU = d.pings = 0;
			y.pingsDAC = y.pingsFFT = y.pingsYOU = y.pings = 0;

			allDevices["svr"].Pulse(allDevices, false, true);

			Console.WriteLine($"SVR -> FFT: {f.pings}");
			Console.WriteLine($"SVR -> YOU: {y.pingsYOU} := {y.pingsDAC} + {y.pingsFFT}");

			return y.pingsFFT;

			// SRV -> FFT - false
			// SRV -> DAC - true
			// SRV -> YOU - true
			// DAC -> FFT - 
			// FFT -> DAC - true
			// DAC -> FFT - 


			// FFT -> DAC - 0
			// FFT -> YOU - true (but does not pass through DAC)
			// SVR -> YOU - true (1912898; mostly early)
			// SVR -> DAC - true (  43232; mostly late)
			// DAC -> YOU - true; 3 (no FFT)

			// SVR -- FFT 
			// SVR -- DAC (over 25000)
			// DAC -- YOU = 3

			// YOU -> OUT does not pass through DAC or FFT; 599
			// SVR --- FFT --- DAC --- YOU --- OUT
			//                      3      599

			/*allDevices["svr"].Pulse(allDevices, false, false);

			long fft = allDevices["fft"].pings;
			allDevices["fft"].Pulse(allDevices, false, true);

			return fft * allDevices["out"].pings;*/
		}
	}
}
