//
using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read input registers functions/requests.
    /// </summary>
    public class ReadInputRegistersFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadInputRegistersFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public ReadInputRegistersFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            ModbusReadCommandParameters p = (ModbusReadCommandParameters)CommandParameters;
            byte[] request = new byte[12];

            WriteUInt16(request, 0, p.TransactionId);
            WriteUInt16(request, 2, p.ProtocolId);
            WriteUInt16(request, 4, p.Length);
            request[6] = p.UnitId;
            request[7] = p.FunctionCode;
            WriteUInt16(request, 8, p.StartAddress);
            WriteUInt16(request, 10, p.Quantity);

            return request;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            ValidateResponse(response);
            ModbusReadCommandParameters p = (ModbusReadCommandParameters)CommandParameters;
            Dictionary<Tuple<PointType, ushort>, ushort> retVal = new Dictionary<Tuple<PointType, ushort>, ushort>();

            for (int i = 0; i < p.Quantity; i++)
            {
                int startIndex = 9 + (i * 2);
                ushort value = ReadUInt16(response, startIndex);
                retVal.Add(new Tuple<PointType, ushort>(PointType.ANALOG_INPUT, (ushort)(p.StartAddress + i)), value);
            }

            return retVal;
        }

        private void ValidateResponse(byte[] response)
        {
            if (response == null || response.Length < 9)
            {
                throw new ArgumentException("Invalid modbus response.");
            }

            byte responseFunctionCode = response[7];
            if ((responseFunctionCode & 0x80) != 0)
            {
                HandeException(response[8]);
            }
        }

        private ushort ReadUInt16(byte[] buffer, int startIndex)
        {
            return (ushort)((buffer[startIndex] << 8) | buffer[startIndex + 1]);
        }

        private void WriteUInt16(byte[] buffer, int startIndex, ushort value)
        {
            buffer[startIndex] = (byte)(value >> 8);
            buffer[startIndex + 1] = (byte)(value & 0xFF);
        }
    }
}
