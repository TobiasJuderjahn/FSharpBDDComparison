Feature: Crontab Tests

  Scenario Outline: Crontab Parsing
    When crontab expression <CrontabExpression> is parsed
    Then the CrontabSchedule has the same string representation

    Examples:
      | CrontabExpression |
      | 0 0 2 * *         |
      | 0 0 2 * * *       |

  Scenario Outline: Running/Not Running Interval is identified successfully
    When isRunning is called at <PointInTime> with the following parameters
      | Parameter               | Value        |
      | startScheduleCrontab    | 0 7 * * 1-5  |
      | stopScheduleCrontab     | 0 16 * * 1-5 |
      | maxStartedDurationHours | 72           |
      | maxStoppedDurationHours | 72           |
    Then the isRunning result is <Result>

    Examples:
      | PointInTime          | Result |
      | 2020-10-21T00:00:00Z | false  |
      | 2020-10-21T08:00:00Z | true   |

  Scenario Outline: Last Run DateTime is retrieved successfully
    When getLastRunOn is called at <PointInTime> with the following parameters
      | Parameter           | Value       |
      | runScheduleCrontab  | 0 0 2 * * * |
      | maxRunIntervalHours | 24          |
    Then the getLastRunOn result is <Result>

    Examples:
      | PointInTime          | Result               |
      | 2020-10-21T00:00:00Z | 2020-10-20T02:00:00Z |
      | 2020-10-21T08:00:00Z | 2020-10-21T02:00:00Z |    

  
  