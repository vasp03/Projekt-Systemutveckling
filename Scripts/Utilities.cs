using System;

namespace Goodot15.Scripts;

public class Utilities {
	public static int timeToTicks(double seconds = 0, double minutes = 0, double hours = 0, double days = 0) {
		return (int)Math.Floor(
			seconds * 60 + // Seconds to ticks
			minutes * 60 * 60 + // Minutes to seconds to ticks
			hours * 60 * 60 * 60 + // Hours to seconds to ticks
			days * 24 * 60 * 60 * 60 // Days to seconds to ticks
		);
	}
}