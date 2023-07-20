# Threat Modeling

## About Threat Dragon
Threat dragon is a design software to model your architecture with respect to the flow of data. Nodes, dataflow, and trust boundaries are key to identifying threats and their risks. Threat dragon will not create test code for you. It will not automate anything. It is simply a way to communicate threats, risks, and mitigations.

### Components to Threat Dragon
* Process - Generally any running code. A micro app, centralized system, etc.
* Store - Data at rest. Configuration, Database, Message Queue, etc.
* Actor - A third party. The third party can be a browser, an attacker, a user. It is not always the origin of a threat, just a component.
* Data Flow - The direction data is flowing. Request/response is too generic. If you specify particular requests (GET), reads are coming from the responder. If you are specifying writes (POST,PUSH,DELETE) it is coming from the requester. This is the direction of data.
* Trust Boundary - The trust boundary is generally created between any two components where data is flowing. A single trust boundary can be created around a component if many components are reaching out to it. It is better to be specific with trust boundaries when you can to help identify the relationship between two components with respect to data flow.

## Identifying threats with STRIDE
Threats can come in many different flavors, but it can be summed up in a few different buckets that make up STRIDE.

* Spoofing - Mocking credentials in order to gain access to data. Spoofing usually involves capturing credentials, or something that fits the expectation of verified credentials to a system.
* Tampering - Altering data or functionality to achieve a goal. In a message queue we would call this message poisoning if a user gained access to the queue such that they can create a message that looks valid to the system, but causes unexpected behavior to the subscriber. Another example would be if DLL was replaced to create a backdoor for later entry.
* Repudiation - The threat is the lack of traceability. Repudiation is often subsequent of other attacks, but can be acknowledged in lew of known threats. The lack of visibility itself is always a threat.
* Information disclosure - Data being exposed to a broader audience than anticipated is a threat. This can be collecting access information for users, discovering PII, or gaining information about a system that is not necessary to perform operations for the audience receiving this information. Being able to lock down all information to specific users is not always possible, but perferred whenever possible.
* Elevation of privilege - Gaining more access than anticipated either through operations that would give more access unexpectedly, or exposure to credentials for a user who already has elevated privilege.

## Threat Traceability matrix
Ideally we can manage all threats at once. There is enough time in the day to clear the log. We can all sleep at night. Unfortunately this is not the case. Threats are ever growing, and it is not from a lack of discipline. It is simply the nature of security. To help us identify order of importance, we need some scale to measure the severity of a threat, but it is not as simply a gut feeling. 

### DREAD
DREAD is a form of measuring through variables that help us form a risk value.
* Damage - how bad would an attack be? 0-10
* Reproducibility - how easy is it to reproduce the attack? 0 - 10
* Exploitability - how much work is it to launch the attack? 0 - 10
* Affected users - how many people will be impacted? Likely use this as a percentage of users, but scaled from 0-10.
* Discoverability - how easy is it to discover the threat? This part is arguably security through obscurity; however, you can consider this attack surface. If you have 0 attack surface, you have 0 discoverability. if you have 5 entry points to this vulnerability, you have 5 points of discoverability. Measuring discoverability is important, but removing discoverability is not a solution. (0-10)

Risk Value = (Damage + Affected users) x (Reproducibility + Exploitability + Discoverability).

#### DREAD Example
Threat: I am an administrator for RedLine. As an administrator, I can grant and revoke activities to individual users, but not myself. For this example, we will assume that our access tokens expire after a year if there is no activity. Refresh expires in one year and one week. We will also assume that the application is not using SSL. We have a man-in-the-middle attack (Information discovery) and now the attacker has a JWT for an adminstrator (Spoofing). Removing all access to all users is possible (Denial of Service). The list goes on, but for simplicity we'll focus on these three.

Information discovery open text communication:
* Damage (assuming one user) - 6 (A token is exposed, but no actions yet).
* Reproducibility - 10 (easy to capture the token in traffic).
* Explotability - 10 (easy to exploit if you're on the network).
* Affected users - 1 (This is only capturing the data).
* Discoverability - 6 (WWW discovery is scanning and analysis. A targeted tap into our network would be extremely easy here without SSL).

Information discovery Risk Value = (6 + 1) x (10 + 10 + 6) = 182

Spoofing through captured token:
* Damage (assuming one user) - 10 (high ranking credentials).
* Reproducibility - 1 (JWT captured, and does not expire for a year, but the JWT can be revoked when discovered).
* Exploitibility - 10 (easy exploit if you have the JWT).
* Affected users - 1 (The one administrator is immediately affected).
* Discoverability - 10 (Information discovery yielded this JWT, it can be spread. JWT is well known in web technology).

Spoofing Risk Value = (10 + 1) x (10 + 1 + 10) = 231

DoS with captured token:
* Damage (assuming one user) - 10 (This is highly).
* Reproducibility - 6 (easy to call commands once you have the credentials).
* Exploitability - 10 (This is a valid operation).
* Affected users - 10 (All the users).
* Discoverability - 10 (by design, the user has the ability to revoke grants at will to any user except their own).

DoS Risk Value = (10 + 10) x (10 + 6 + 10) = 520

## Ranking the Risk
As we can see, the most severe scenario here is the DoS. Spoofing and information discovery are nothing to ignore, but we'll see in the attack tree that even though DoS is the greater threat, it is not the threat to solve directly.

### Attack Tree
Now that we are looking at the risk values, and we see DoS is a huge problem here, let's investigate how DoS can happen. And the quickest conclusion is that because we have an information discovery threat that allows capturing a token of a user. surface level exploration in the application, the attacker can see that the user is possibly an administrator and begins granting and revoking actions/groups starting with the most privileged.

```
                            [Exposed JWT](Information disclosure)
                            /                       \
            [Administrator] (Spoof)                 [User with PII Access] (Spoof)
                /           \                                                   \
[Grant Revocations] (DoS)   [Create more administrators] (Elevated privilege)   [PII Legal Problems] (information disclosure)
```

## Mitigations
The strategy for a mitigation is dependent on the threat, but they generally fall into Reduce, Transfer, Avoid, or Accept. When identifying a mitigation, we should be able to associate at least one of these buckets with the solution.

Reduce: build a solution with the threat in mind to eliminate most of the higher risk items of the threat.

### Tool
[Threat Dragon](http://docs.threatdragon.org/#downloads)
### Reference
[OWASP Cheatr Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Threat_Modeling_Cheat_Sheet.html)

[Defining STRIDE](https://www.softwaresecured.com/stride-threat-modeling/)

[PASTA](https://owasp.org/www-pdf-archive/AppSecEU2012_PASTA.pdf)