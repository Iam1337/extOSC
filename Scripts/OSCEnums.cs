/* Copyright (c) 2020 ExT (V.Sigalkin) */

using System;

namespace extOSC
{
	public enum OSCValueType
	{
		Unknown,

		// Convertable
		Int,     // Tag: i
		Long,    // Tag: h
		True,    // Tag: T
		False,   // Tag: F
		Float,   // Tag: f
		Double,  // Tag: d
		String,  // Tag: s
		Null,    // Tag: N
		Impulse, // Tag: I
		Char,    // Tag: c (char 32bit)
		Color,   // Tag: r (RGBA 32bit)
		Blob,    // Tag: b (byte array - osc)
		TimeTag, // Tag: t (osc-time tag)
		Midi,    // Tag: m (MIDI O_o)
		Array    // Tag: "[" and "]"
	}

	[System.Flags]
	public enum OSCReflectionType
	{
		Unknown = 0x00,
		Field = 0x01,
		Property = 0x02,
		Method = 0x04,
		All = Field | Property | Method
	}

	public enum OSCReflectionAccess
	{
		Any,
		Read,
		Write,
		ReadWrite
	}

	public enum OSCLocalHostMode
	{
		Any,
		Custom
	}

	public enum OSCLocalPortMode
	{
		FromRemotePort,
		FromReceiver,
		Random,
		Custom
	}

	[Flags]
	public enum OSCSendOptions
	{
		None = 0,
		IgnoreBundle = 1 << 0,
		IgnoreMap = 1 << 1,
	}
}