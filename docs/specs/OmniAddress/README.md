# OmniAddress <!-- omit in toc -->

## Table of Contents <!-- omit in toc -->

- [TCP address format](#tcp-address-format)
  - [EBNF](#ebnf)
  - [Example](#example)
- [References](#references)

## TCP address format

### EBNF

```BNF
<tcp> = 'tcp(' <host> [ ',' <port> ]* ')'
<host> = <ip4> | <ip6> | <dns>
<ip4> = 'ip4(' ? IPv4 address ? ')'
<ip6> = 'ip6(' ? IPv6 address ? ')'
<dns> = 'dns(' ? Domain name ? ')'
<port> = ? Number from 0 to 65535 ?
```

### Example

+ tcp(ip4(127.0.0.1),80)
+ tcp(ip6(::1),443)
+ tcp(dns(localhost),80)
+ tcp(ip4(192.168.0.1))

## References

+ <https://github.com/multiformats/multiaddr/blob/master/protocols.csv>
