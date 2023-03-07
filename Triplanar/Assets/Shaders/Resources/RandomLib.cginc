// The below macro is used to get a random number which varies across different generations. 
#define rnd(seed, constant)  wang_rnd(seed +triple32(_session_rand_seed) * constant) 


uint triple32(uint x)
{
      x ^= x >> 17;
      x *= 0xed5ad4bbU;
      x ^= x >> 11;
      x *= 0xac4c1b51U;
      x ^= x >> 15;
      x *= 0x31848babU;
      x ^= x >> 14;
      return x;
}

float wang_rnd(uint seed)
{
      uint rndint = triple32(seed);
      return ((float)rndint) / float(0xFFFFFFFF);             // 0xFFFFFFFF is max unsigned integer in hexa decimal
}