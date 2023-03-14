using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum TemperatureType
{
    J = 0, // Iron-constantan (Type J) thermocouple
    K = 1, // Chromel-Alumel (Type K) thermocouple
    T = 2, // Copper-Constantan (Type T) thermocouple
    E = 3, // Chromel-Constantan (Type E) thermocouple
    R = 4, // Pt13%Rh-Pt (Type R) thermocouple
    S = 5, // Pt10%Rh-Pt (Type S) thermocouple
    B = 6, // Pt30%Rh-Pt6%Rh (Type B) thermocouple
    N = 7, // Nicrosil-Nisil (Type N) thermocouple
    C = 8, // W5%Re-W26%Re (Type C) thermocouple
    L = 9, // Cu10%Ni-Fe50%Ni (Type L) thermocouple
    U = 10 // user-defined thermocouple
}
