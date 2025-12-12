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

			public readonly List<string> Inputs;

			public int pings = 0;
			public int pongs = 0;

			public Device(string line)
			{
				string[] parts = line.Split(':');
				ID = parts[0];
				Outputs = parts[1].Split(' ').Where(s => !string.IsNullOrEmpty(s)).ToArray();
				Inputs = new List<string>();
			}

			// for part 1
			public void Pulse(Dictionary<string, Device> allDevices)
			{
				pings++;
				foreach (string o in Outputs)
				{
					allDevices[o].Pulse(allDevices);
				}
			}

			// for part 2
			public void Pong(Dictionary<string, Device> allDevices, int depth)
			{
				pongs++;
				foreach (string o in Inputs)
				{
					allDevices[o].Pong(allDevices, depth +1);
				}
			}

			// for part 2
			public void Touch(Dictionary<string, Device> allDevices)
			{
				foreach (string o in Outputs)
				{
					allDevices[o].Inputs.Add(ID);
				}
			}

			public override string ToString()
			{
				return ID;
			}
		}

		internal static long Part1(string input)
		{
			return -1;
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

			Dictionary<(Device d, bool dac, bool fft), (int depth, long pings)> pings = new Dictionary<(Device d, bool dac, bool fft), (int depth, long pings)>();
			List<(Device d, bool dac, bool fft)> openSet = new List<(Device d, bool dac, bool fft)>();
			openSet.Add((allDevices["svr"], false, false));
			pings.Add((allDevices["svr"], false, false),(0, 1));

			while (openSet.Any())
			{
				(Device d, bool dac, bool fft) d = openSet[0];
				openSet.RemoveAt(0);
				foreach (var o in d.d.Outputs)
				{
					Device d2 = allDevices[o];

					(int d, long p) v = pings[d];
					bool dac = d.dac || d2.ID == "dac";
					bool fft = d.fft || d2.ID == "fft";

					if (d2.ID == "out" && !dac && !fft)
					{
						continue;
					}

					var k = (d2, dac, fft);
					pings.TryAdd(k, (v.d+1, 0));

					long p1 = pings[k].pings;
					long p2 = v.p;

					pings[k] = (pings[k].depth, p1 + p2);
					openSet.Add(k);
				}
				if(d.d.ID != "out")
					pings[d] = (pings[d].depth, 0);
				openSet.RemoveAll(s => pings[s].pings == 0);
				openSet.Sort((a, b) =>
				{
					if (pings[a].pings == 0 && pings[b].pings > 0) return 1;
					if (pings[b].pings == 0 && pings[a].pings > 0) return -1;
					int c = pings[a].depth.CompareTo(pings[b].depth);

					if (c == 0)
					{
						if ((a.dac || a.fft) && !(b.dac || b.fft)) return -1;
						if (!(a.dac || a.fft) && (b.dac || b.fft)) return 1;
					}

					return c;
				});
			}

			return pings[(allDevices["out"], true, true)].pings;

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
