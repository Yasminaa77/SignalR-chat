import { useState, useEffect, useRef } from "react";

import "./App.css";

import useSignalR from "./useSignalR";

type Message = {
  id: number;
  text: string;
  userName: string;
  created: Date;
  channelId: number;
  // channel?: Channel;
};

export default function App() {
  const messageListRef = useRef<HTMLDivElement>(null);
  const { connection } = useSignalR("/r/chat");
  const [input, setInput] = useState("");
  const [roomNum, setRoomNum] = useState(1);
  const [editMode, setEditMode] = useState(0);
  const [user , setUser] = useState("")

  const [messages, setMessages] = useState<Message[]>([]);
  const [channels, setChannels] = useState<{ id: number; name: string }[]>([]);

  //004027

  useEffect(() => {
    if (!connection) {
      return;
    }
       const handleUser = () => {
      const name = window.prompt("Please enter your name:");
      if (name) {
        sessionStorage.setItem('username', name); // Store user name in session
        //console .log the sessionvalue to see if it works
        console.log(sessionStorage.getItem('username'));
       
      } else {
        handleUser();
      }
    };

    //if the session is empty ask for the username
    sessionStorage.getItem('username') === null && handleUser()

  

    //only listen to the messages coming from certain chat room
    connection.invoke("AddToGroup", roomNum.toString());
    console.log("roomNum: " + roomNum);

    //Recieve/listen for messages from the server
    connection.on("ReceiveMessage", (message: Message) => {
      console.log(message);
      message.created = new Date(message.created);
      setMessages((messages) => [...messages, message]);
      //  console.log("from the server "+  message);
    });


    connection.on("MessageDeleted", (messageId: number) => {
      setMessages(messages.filter((message) => message.id !== messageId));
    });
   

    fetch("/api/channels", { method: "GET" })
      .then((response) => response.json())
      .then((data) => {
        console.log(data);
        setChannels(data);
      });

    fetch(`/api/channels/${roomNum}/messages`, { method: "GET" })
      .then((response) => response.json())
      .then((data) => {
        const messages = data.map((message: any) => {
          message.created = new Date(message.created);
          return message;
        });
        setMessages(messages);
      });
    

    return () => {
      connection.invoke("RemoveFromGroup", roomNum.toString());
      connection.off("ReceiveMessage");
    };
  }, [connection, roomNum]);


//handle submit
  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    console.log(roomNum);
    await fetch(`/api/channels/${roomNum}/messages`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        text: input,
        //give me the code to set the use name equals to  session  usernamw 
        // username: sessionStorage.getItem('username'),
      
        // userName: user,
        username: "j",

      }),
    });

    setInput("");
  };


//delete message
  const handleDelete = async (id: number) => {
    await fetch(`/api/messages/${id}`, {
      method: "DELETE",
    });
    setMessages(messages.filter((message) => message.id !== id));
  };



  return (
    <div className="App">
      {/* SECTION LEFT  */}

      <div className="sectionLeft">
        <div className="header">
          <h3>SignalR Chat</h3>
          {/* write me the coe ethat say hi and the nem in session usernae */}
          <h4>Hi {sessionStorage.getItem('username')}</h4>
          <p>You are {connection ? "connected" : "Not Connected"}</p>
        </div>
        <h4>Channels</h4>
        <ul className="channelList">
          {channels.map((channel) => (
            <li
              key={channel.id}
              className={roomNum === channel.id ? "active" : ""}
              onClick={() => setRoomNum(channel.id)}
            >
              {channel.name}
              <div className="divider"></div>
            </li>
          ))}
        </ul>
        <button onClick={() => {
  sessionStorage.removeItem('username');
  window.location.reload();
}}>Logout</button>



      </div>

      {/* SECTION RIGHT */}

      <div className="sectionRight">
        {/* filter the messages by the channel id */}
            <div className="cards" ref={messageListRef}>
      {messages
        .filter((message) => message.channelId === roomNum)
        .map((message) => (
          <div key={message.id} className="message-card">
            <span className="close" onClick={() => handleDelete(message.id)}>
              ×
            </span>
            <p className="username">{message.userName}</p>
            <h3>{message.text}</h3>
            <p className="timestamp">{message.created.toString()}</p>
          </div>
        ))}
    </div>


        {/* form to send messages */}
        <div className="form">
          <form onSubmit={handleSubmit}>
            <input
              type="text"
              value={input}
              onChange={(e) => setInput(e.target.value)}
            />
            <button type="submit">Send</button>
          </form>
        </div>
      </div>
    </div>
  );
}

